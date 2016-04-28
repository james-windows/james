#ifndef TESTING
	#pragma once
#endif

#include <cstdio>
#include <algorithm>
#include <vector>
#include <cstring>
#include <string>
#include <iostream>
#include <fstream>
#include <functional>
#include <set>
#include <array>
#include <map>
#include <stack>
#include <list>

using namespace std;

#define all(a) (a).begin(),(a).end()
#define pb push_back
#define sz(a) ((int)(a).size())
#define mp make_pair
#define fi first
#define se second

typedef pair<int,int> pint;
typedef long long ll;
typedef vector<int> vi;

const int MAX_SIZE=10;

#ifdef _WIN32
	_declspec(dllexport) const char *SearchEncodeLower(const char *s);
	#define strcasecmp _stricmp
	const char seperator='\\';
	#pragma warning(disable : 4996)
#else
	const char seperator='/';
#endif

#ifdef TESTING
	//usually implemented in the objective-c++/c# wrapper
	//decodes the given string, applies lower case and decodes it again
	//no encoding/decoding needed for testing purposes
	const char *SearchEncodeLower(const char *a)
	{
		int l=strlen(a);
		char *r=(char *)malloc(sizeof(char)*(l+1));
		r[l]='\0';
		for (int i=0; i<l; i++)
			r[i]=tolower(a[i]);
		return r;
	}
#endif

#define strlen(a) ((int)strlen(a))

struct cmp_str 
{
	bool operator()(const char *a, const char *b) const
	{
		return strcasecmp(a,b)<0;
	}
};

struct pathNode;

typedef map<const char*,pathNode*,cmp_str> pathChilds;

struct pathNode 
{
	const char *s;
	pathNode *p;
	pathChilds childs;
	pathNode(const char *s, pathNode *p=0):s(s),p(p){}

	bool incomplete()
	{
		if (!p)
			return s!=0;
		return p->incomplete();
	}

	char *path()
	{
		int len=0;
		stack<pathNode*> st;
		for (pathNode *here=this; here->p; here=here->p)
		{
			st.push(here);
			len+=strlen(here->s)+1;
		}
		char *ans=new char[len];
		char *there=ans;
		for (;!st.empty(); st.pop())
		{
			for (const char *s=st.top()->s; *s; s++,there++)
				*there=*s;
			*there=seperator;
			there++;
		}
		there--;
		*there='\0';
		return ans;
	}

	//returns true iff the current pathNode represents the given string
	//we could also compare the output of path(), but path() is allocating memory
	bool comparePath(const char *p)
	{
		pathNode *here=this;
		stack<pathNode*> st;
		for (;here->p; here=here->p)
			st.push(here);
		bool work=true;
		for (;!st.empty()&&work; st.pop())
		{
			const char *s=st.top()->s;
			for (; *p&&*s&&work; s++,p++)
				work=tolower(*s)==tolower(*p);
			if (*p)
			{
				work&=*p==seperator;
				p++;
			}
			else
				work&=(*s=='\0');
		}
		return work&&(*p=='\0');
	}

	//kills this node and returns a copy of the unassigned child pointers
	pathChilds remove()
	{
		if (p)
		{
			p->childs.erase(s);
			for (auto it : childs)
				it.se->p=0;
			pathChilds ans=childs;
			return ans;
		}
		return pathChilds();
	}

	void rename(const char *_new)
	{
		p->childs.erase(s);
		s=_new;
		p->childs[s]=this;
	}
};

class PathManager
{
private:
	pathNode *root;
	
	pathNode *insert(pathNode *n, const char *s)
	{
		if (n->childs.find(s)!=n->childs.end())
			return n->childs[s];
		pathNode *nn=new pathNode(s,n);
		n->childs[s]=nn;
		return nn;
	}
	
	void split(const char *path, bool free, function<void(const char *)> callback)
	{
		int l=strlen(path);
		char *np=new char[l+(path[l-1]!=seperator)];
		char *p=np;
		for (int i=0; i<l; i++)
			if (path[i]!=seperator)
				np[i]=path[i];
			else
			{
				np[i]='\0';
				callback(p);
				p=np+i+1;
			}
		if (path[l-1]!=seperator)
		{
			np[l]='\0';
			callback(p);
		}
		if (free)
			delete[] np;
	}

	pathNode *find(const char *path)
	{
		pathNode *last=root;
		split(path,true,[&](const char *s) {
			if (last&&last->childs.find(s)!=last->childs.end())
				last=last->childs[s];
			else
				last=0;
		});
		return last;
	}

	void print(pathNode *n, int t=0)
	{
		for (int i=0; i<t; i++)
			printf("  ");
		printf("%s/\n",n->s);
		for (auto it : n->childs)
			print(it.se,t+1);
	}

	void rec(pathNode *here, list<const char*> &out)
	{
		out.pb(here->path());
		for (auto it : here->childs)
			rec(it.se,out);
	}

public:
	PathManager()
	{
		root=new pathNode(0);
	}
	
	pathNode *insert(const char *path)
	{
		pathNode *prev=root;
		split(path,false,[&](const char *s) {
			prev=insert(prev,s);
		});
		return prev;
	}

	//remove the folder at 'path', but none of its childs
	pathChilds remove(const char *path)
	{
		pathNode *here=find(path);
		if (here)
			return here->remove();
		return pathChilds();
	}

	pathNode *rename(const char *old, const char *_new)
	{
		pathChilds lonely=remove(old);
		pathNode *father=insert(_new);
		//assuming uniqueness
		for (auto it : lonely)
		{
			father->childs[it.fi]=it.se;
			it.se->p=father;
		}
		return father;
	}

	void print()
	{
		for (auto it : root->childs)
			print(it.se);
	}

	void getSubtree(const char *path, list<const char*> &out)
	{
		pathNode *here=find(path);
		if (here)
			rec(here,out);
	}
};

struct metadata
{
	int id,prio;
	pathNode *file;
	metadata(int prio=0):prio(prio)
	{
		static int rid=0;
		id=rid++;
	}
};

struct node
{
	int len;
	char *val;
	node *next,*child,*par;
	int nres;
	vector<metadata*> data;
	metadata **results;
	node(int cnt, const char *nval, int l)
	{
		results=new metadata*[cnt];
		nres=0;
		next=child=par=0;
		len=l;
		val=new char[l];
		strncpy(val,nval,l);
	}
	~node()
	{
		delete[] val;
		delete[] results;
		data.clear();
	}
};

class SearchEngine
{
private:
	const int num_results;
	node *root=0;
	PathManager paths;
	
	int compare(const char *a, int l1, const char *b, int l2) const
	{
		int lo=min(l1,l2);
		for (int i=0; i<lo; i++)
			if (a[i]!=b[i])
				return i;
		return lo;
	}
	
	void merge(node *data, node *n)
	{
		if (!data||!n)
			return;
		static metadata *tmp[MAX_SIZE];
		int p1=0,p2=0,p3=0;
		for (; p3<num_results&&p1<data->nres&&p2<n->nres; )
		{
			if (p3>0&&data->results[p1]->id==tmp[p3-1]->id)
			{
				p1++;
				continue;
			}
			if (p3>0&&n->results[p2]->id==tmp[p3-1]->id)
			{
				p2++;
				continue;
			}
			if (data->results[p1]->prio>n->results[p2]->prio||(data->results[p1]->prio==n->results[p2]->prio&&data->results[p1]->id<n->results[p2]->id))
				tmp[p3]=data->results[p1++];
			else
				tmp[p3]=n->results[p2++];
			p3++;
		}
		for (; p3<num_results&&p1<data->nres; p1++)
			if (!(p3>0&&data->results[p1]->id==tmp[p3-1]->id))
				tmp[p3++]=data->results[p1];
		for (; p3<num_results&&p2<n->nres; p2++)
			if (!(p3>0&&n->results[p2]->id==tmp[p3-1]->id))
				tmp[p3++]=n->results[p2];
		data->nres=p3;
		for (int i=0; i<p3; i++)
			data->results[i]=tmp[i];
	}
	
	void put(node *res, metadata *n)
	{
		if (!res||!n)
			return;
		for (int i=0; i<res->nres; i++)
			if (res->results[i]->prio<n->prio||(res->results[i]->prio==n->prio&&res->results[i]->id>n->id))
			{
				res->nres=min(num_results,res->nres+1);
				for (int j=res->nres-1; j>i; j--)
					res->results[j]=res->results[j-1];
				res->results[i]=n;
				return;
			}
			else if (res->results[i]->id==n->id)
				return;
		if (res->nres<num_results)
			res->results[res->nres++]=n;
	}
	
	int insert(node *&n, const char *s, int l, metadata *data)
	{
		if (!n)
		{
			n=new node(num_results,s,l);
			n->data.pb(data);
			put(n,data);
			return 0;
		}
		int k=compare(n->val,n->len,s,l);
		if (k==0)
		{
			insert(n->next,s,l,data);
			n->next->par=n->par;
			merge(n->par,n->next);
			return 0;
		}
		if (k<l)
		{
			if (k<n->len)
			{
				node *nn=new node(num_results,n->val+k,n->len-k);
				nn->child=n->child;
				if (nn->child)
					nn->child->par=nn;
				nn->par=n;
				n->child=nn;
				delete[] n->val;
				n->val=new char[k];
				strncpy(n->val,s,k);
				n->len=k;
				nn->nres=n->nres;
				nn->data=n->data;
				for (int i=0; i<n->nres; i++)
					nn->results[i]=n->results[i];
				n->data.clear();
			}
			insert(n->child,s+k,l-k,data);
			n->child->par=n;
			merge(n,n->child);
			return 0;
		}
		metadata *old=0;
		for (int i=0; !old&&i<sz(n->data); i++)
			if (n->data[i]->file==data->file)
				old=n->data[i];
		if (!old)
		{
			n->data.pb(data);
			put(n,data);
			return 0;
		}
		return data->prio-old->prio;
	}
	
	void remerge(node *n)
	{
		n->nres=0;
		for (int i=0; i<sz(n->data); i++)
			put(n,n->data[i]);
	}
	
	//does not clear search path
	//call (return value)->remove() afterwards
	metadata *remove(node *&n, const char *s, int l, const char *p)
	{
		if (!n)
			return 0;
		int k=compare(n->val,n->len,s,l);
		if (k==l)
		{
			metadata *ans=0;
			bool did=false;
			for (vector<metadata*>::iterator it=n->data.begin(); it!=n->data.end();)
			{
				bool icp=(*it)->file->incomplete();
				if (icp||(*it)->file->comparePath(p))
				{
					did=true;
					metadata *data=*it;
					it=n->data.erase(it);
					if (!icp)
						ans=data;
					else
						data->file->remove();
				}
				else
					it++;
			}
			if (did)
				remerge(n);
			if (!sz(n->data))
			{
				node *nn=n->next;
				delete n;
				n=nn;
			}
			return ans;
		}
		if (k==0)
			return remove(n->next,s,l,p);
		if (k==n->len)
		{
			metadata *ans=remove(n->child,s+k,l-k,p);
			if (n->child&&!n->child->next)
			{
				node *nn=n->child;
				char *nval=new char[n->len+nn->len];
				strncpy(nval,n->val,n->len);
				strncpy(nval+n->len,nn->val,nn->len);
				delete n->val;
				n->val=nval;
				n->len+=nn->len;
				n->child=nn->child;
				for (int i=0; i<sz(nn->data); i++)
					n->data.pb(nn->data[i]);
				delete nn;
			}
			remerge(n);
			for (node *nn=n->child; nn; nn=nn->next)
				merge(n,nn);
			return ans;
		}
		return 0;
	}
	
	void generateSplits(const char *name, function<void(const char *)> callback) const
	{
		static const char *cake=" ._-,";
		callback(name);
		//for (int i=1; name[i]; i++)
		//	if (isupper(name[i]))
		//		callback(name+i);
		//	else if (isalnum(name[i+1]))
		//	{
		//		bool match=false;
		//		int j=0;
		//		for (; !match&&cake[j]; j++)
		//			match=name[i]==cake[j];
		//		if (match)
		//		{
		//			callback(name+i+1);
		//			if (j==1)
		//				callback(name+i);
		//			i++;
		//		}
		//	} else if (name[i]=='%') //encoded
		//		i+=2;
	}
	
	int addPrio(node *n, const char *s, int l, const char *p, int v)
	{
		if (!n)
			return 0;
		int k=compare(n->val,n->len,s,l);
		if (k==l)
		{
			for (auto it : n->data)
				if (it->file->comparePath(p))
				{
					it->prio+=v;
					remerge(n);
					return it->prio;
				}
			return 0;
		}
		if (k==0)
			return addPrio(n->next,s,l,p,v);
		if (k==n->len)
		{
			int ans=addPrio(n->child,s+k,l-k,p,v);
			remerge(n);
			for (node *nn=n->child; nn; nn=nn->next)
				merge(n,nn);
			return ans;
		}
		return 0;
	}
	
	metadata *makeMetadata(const char *s, int prio)
	{
		metadata *data=new metadata(prio);
		data->file=paths.insert(s);
		return data;
	}
	
	void dfs(node *n, ofstream &out, set<int> &s) const
	{
		if (!n->child)
		{
			for (metadata *m : n->data)
				if (s.insert(m->id).se)
				{
					const char *file=m->file->path();
					out<<file<<";"<<m->prio<<"\n";
					delete file;
				}
		}
		else
			dfs(n->child,out,s);
		if (n->next)
			dfs(n->next,out,s);
	}
	
	//splits str - saves the last part into file, converted to lower case (using decode/encode methods)
	void splitFileStr(const char *str, const char **file)
	{
		int last=-1;
		for (int i=0; str[i]; i++)
			if (str[i]==seperator)
				last=i;
		*file=SearchEncodeLower(str+last+1);
	}

	int insert(metadata *data, const char *name)
	{
		int old=0;
		generateSplits(name,[&](const char *s) {
			old=insert(root,s,strlen(s)+1,data);
		});
		return old;
	}

public:
	SearchEngine(int results):num_results(results)
	{
		if (num_results>MAX_SIZE)
		{
			fprintf(stderr,"size %d > MAX_SIZE (%d)\n",num_results,MAX_SIZE);
			exit(1);
		}
	}

	//str: encoded
	//gets split by last occurrence of 'seperator'
	void insert(const char *str, int prio)
	{
		const char *name;
		splitFileStr(str,&name);
		metadata *mt=makeMetadata(str,prio);
		int delta=insert(mt,name);
		if (delta)
		{
			delete mt;
			addPriority(str,delta);
		}
	}

	//str, as with insert
	metadata *remove(const char *str, bool kill=true)
	{
		const char *name;
		splitFileStr(str,&name);
		metadata *ans=0;
		generateSplits(name,[&](const char *s) {
			ans=remove(root,s,strlen(s)+1,str);
		});
		if (kill&&ans)
		{
			if (ans->file)
				ans->file->remove();
			delete ans;
		}
		return ans;

	}

	void removeRecursive(const char *str)
	{
		list<const char*> files;
		paths.getSubtree(str,files);
		for (auto file : files)
			remove(file);
		files.clear();
	}

	void rename(const char *old, const char *_new)
	{
		metadata *d=remove(old,false);
		if (d)
		{
			d->file=paths.rename(old,_new);
			const char *name;
			splitFileStr(_new,&name);
			insert(d,name);
		}
		else
			paths.rename(old,_new);
	}
	
	//s = query string, lowercase
	const pair<const char*,int> *find(const char *s) const
	{
		static pair<const char*,int> ans[MAX_SIZE+1];
		if (s[0]=='\0')
		{
			ans[0]=mp((const char*)0,0);
			return ans;
		}
		int l=strlen(s);
		node *n=root;
		while (n)
		{
			int k=compare(n->val,n->len,s,l);
			if (k==l)
			{
				for (int i=0; i<n->nres; i++)
					ans[i]=mp(n->results[i]->file->path(),n->results[i]->prio);
				ans[n->nres]=mp((const char*)0,0);
				return ans;
			}
			if (k==0)
				n=n->next;
			else if (k==n->len)
			{
				n=n->child;
				s+=k;
				l-=k;
			} else
				break;
		}
		ans[0]=mp((const char*)0,0);
		return ans;
	}
	
	void addPriority(const char *str, int v)
	{
		if (v==0)
			return;
		const char *name;
		splitFileStr(str,&name);
		int res=0;
		generateSplits(name,[&](const char *s) {
			res=addPrio(root,s,strlen(s)+1,str,v);
			v=0;
		});
		if (res<0)
			remove(str);
	}
		
	void save(const char *file) const
	{
		ofstream out;
		out.open(file);
		if (root)
		{
			set<int> s;
			dfs(root,out,s);
		}
		out.flush();
		out.close();
	}
	
	static SearchEngine *load(const char *file, int results)
	{
		SearchEngine *search=new SearchEngine(results);
		ifstream in;
		in.open(file);
		string s;
		while (getline(in,s))
		{
			int pos=(int)s.rfind(';');
			int prio=0;
			for (int i=pos+1; i<sz(s); i++)
				prio=prio*10+s[i]-'0';
			string file=s.substr(0,pos);
			const char *encoded=file.c_str();
			search->insert(encoded, prio);
		}
		in.close();
		return search;
	}
};

#ifdef TESTING
int main()
{
	auto search=new SearchEngine(10);
	vector<string> v;
	for (int i=0; i<14248; i++)
	{
		string s;
		getline(cin,s);
		v.pb(s);
		search->insert(s.c_str(),rand());
	}
	random_shuffle(all(v));
	for (auto s : v)
		search->removeRecursive(s.c_str()); 
	search->save("whatever");
	return 0;
}
#endif
