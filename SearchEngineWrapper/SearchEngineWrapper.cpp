#include "Stdafx.h"
#include "SearchEngineWrapper.h"
#include "..\SearchEngine\SearchAlgorithm\search-engine.cpp"

#pragma warning(disable : 4477)

using namespace System::Runtime::InteropServices;
using namespace System::Net;

SearchEngineWrapper::SearchEngineWrapper::SearchEngineWrapper(String^ indexFile)
{
	searchEngine = SearchEngine::load(ConvertToChar(indexFile));
	searchResults = gcnew List<SearchResult^>();
}

void SearchEngineWrapper::SearchEngineWrapper::Save(String^ indexFile) {
	searchEngine->save(ConvertToChar(indexFile));
}

void SearchEngineWrapper::SearchEngineWrapper::Remove(String^ path) {
	int lastSeparator = path->LastIndexOf('\\');
	char* argumentPath = ConvertToChar(path->Substring(0, lastSeparator + 1));
	char* argumentName = ConvertToChar(path->Substring(lastSeparator + 1)->ToLower());
	searchEngine->remove(argumentPath, argumentName);
}

void SearchEngineWrapper::SearchEngineWrapper::Insert(String^ path, int priority) {
	searchEngine->insert(ConvertToChar(path), ConvertToChar(path->ToLower()), priority);
}

void SearchEngineWrapper::SearchEngineWrapper::AddPriority(String^ path, int delta) {
	int lastSeparator = path->LastIndexOf('\\');
	char* argumentPath = ConvertToChar(path->Substring(0, lastSeparator + 1));
	char* argumentName = ConvertToChar(path->Substring(lastSeparator + 1)->ToLower());
	searchEngine->addPriority(argumentPath, argumentName, delta);
}

void SearchEngineWrapper::SearchEngineWrapper::Rename(String^ oPath, String^ nPath) {
	int lastSeparator = oPath->LastIndexOf('\\');
	char* oldPath = ConvertToChar(oPath->Substring(0, lastSeparator + 1));
	char* oldName = ConvertToChar(oPath->Substring(lastSeparator + 1)->ToLower());
	
	lastSeparator = nPath->LastIndexOf('\\');
	char* newName = ConvertToChar(nPath->Substring(lastSeparator + 1)->ToLower());
	searchEngine->rename(oldPath, oldName, ConvertToChar(nPath), newName);
}

void SearchEngineWrapper::SearchEngineWrapper::Find(String^ file) {	
	const array<const char*, 2> *res = searchEngine->find(ConvertToChar(file->ToLower()));
	searchResults->Clear();
	for (int i = 0; res[i][0]; i++)
	{
		SearchResult^ tmp = gcnew SearchResult;
		tmp->path = gcnew String(res[i][0]);
		tmp->path = WebUtility::HtmlDecode(tmp->path);
		tmp->name = gcnew String(res[i][1]);
		tmp->name = WebUtility::HtmlDecode(tmp->name);
		searchResults->Add(tmp);
	}
}

//private functions:
char* SearchEngineWrapper::SearchEngineWrapper::ConvertToChar(System::String^ text) {
	String^ encodedString = WebUtility::HtmlEncode(text);
	char *cNow = new char[encodedString->Length];
	sprintf(cNow, "%s", encodedString);
	return cNow;
}

__declspec(dllexport) const char *SearchEncodeLower(const char *s)
{
	String^ decodedString = WebUtility::HtmlDecode(gcnew String(s));
	decodedString = decodedString->ToLower();
	String^ encodedString = WebUtility::HtmlEncode(decodedString);
	char *cNow = new char[encodedString->Length];
	sprintf(cNow, "%s", encodedString);
	return cNow;
}