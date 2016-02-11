#include "Stdafx.h"
#include "SearchEngineWrapper.h"
#include "..\SearchEngine\shared\SearchAlgorithm\search-engine.cpp"

#pragma warning(disable : 4477)

using namespace System::Runtime::InteropServices;
using namespace System::Net;

SearchEngineWrapper::SearchEngineWrapper::SearchEngineWrapper(String^ indexFile, int results)
{
	this->indexFile = indexFile;
	searchEngine = SearchEngine::load(ConvertToChar(indexFile), results);
	searchResults = gcnew List<SearchResult^>();
}

void SearchEngineWrapper::SearchEngineWrapper::Save() {
	searchEngine->save(ConvertToChar(this->indexFile));
}

void SearchEngineWrapper::SearchEngineWrapper::Remove(String^ path) {
	searchEngine->remove(ConvertToChar(path));
}

void SearchEngineWrapper::SearchEngineWrapper::RemoveRecursive(String^ path) {
	searchEngine->remove(ConvertToChar(path));
}

void SearchEngineWrapper::SearchEngineWrapper::Insert(String^ path, int priority) {
	searchEngine->insert(ConvertToChar(path), priority);
}

void SearchEngineWrapper::SearchEngineWrapper::AddPriority(String^ path, int delta) {
	searchEngine->addPriority(ConvertToChar(path), delta);
}

void SearchEngineWrapper::SearchEngineWrapper::Rename(String^ oPath, String^ nPath) {
	searchEngine->rename(ConvertToChar(oPath), ConvertToChar(nPath));
}

void SearchEngineWrapper::SearchEngineWrapper::Find(String^ file) {	
	const pair<const char*, int> *res = searchEngine->find(ConvertToChar(file->ToLower()));
	//const array<const char*, 2> *res = searchEngine->find(ConvertToChar(file->ToLower()));
	searchResults->Clear();
	for (int i = 0; res[i].first; i++)
	{
		SearchResult^ tmp = gcnew SearchResult;
		tmp->path = gcnew String(res[i].first);
		tmp->path = WebUtility::HtmlDecode(tmp->path);
		tmp->priority = res[i].second;
		searchResults->Add(tmp);
	}
}

//private functions:
char* SearchEngineWrapper::SearchEngineWrapper::ConvertToChar(System::String^ text) {
	String^ encodedString = WebUtility::UrlEncode(text)->Replace("+", " ")->Replace("%5C", "\\")->Replace("%3A", ":");
	return (char*)(Marshal::StringToHGlobalAnsi(encodedString)).ToPointer();
}

__declspec(dllexport) const char *SearchEncodeLower(const char *s)
{
	String^ decodedString = WebUtility::UrlDecode(gcnew String(s));
	decodedString = decodedString->ToLower();
	String^ encodedString = WebUtility::UrlEncode(decodedString)->Replace("+", " ")->Replace("%5C", "\\");
	char *cNow = new char[encodedString->Length];
	sprintf(cNow, "%s", encodedString);
	return cNow;
}