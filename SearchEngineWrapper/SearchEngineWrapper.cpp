#include "Stdafx.h"
#include "SearchEngineWrapper.h"
#include "..\SearchEngine\SearchAlgorithm\search-engine.cpp"
using namespace System::Runtime::InteropServices;

SearchEngineWrapper::SearchEngineWrapper::SearchEngineWrapper(System::String^ indexFile)
{
	searchEngine = SearchEngine::load(ConvertToChar(indexFile));
	searchResults = gcnew List<SearchResult^>();
}

void SearchEngineWrapper::SearchEngineWrapper::Save(System::String^ indexFile) {
	searchEngine->save(ConvertToChar(indexFile));
}

void SearchEngineWrapper::SearchEngineWrapper::Remove(System::String^ path) {
	int lastSeparator = path->LastIndexOf('\\');
	searchEngine->remove(ConvertToChar(path->Substring(0, lastSeparator + 1)), ConvertToChar(path->Substring(lastSeparator + 1)));
}

void SearchEngineWrapper::SearchEngineWrapper::Insert(System::String^ path, int priority) {
	searchEngine->insert(ConvertToChar(path), priority);
}

void SearchEngineWrapper::SearchEngineWrapper::AddPriority(System::String^ path, int delta) {
	int lastSeparator = path->LastIndexOf('\\');
	searchEngine->addPriority(ConvertToChar(path->Substring(0, lastSeparator + 1)), ConvertToChar(path->Substring(lastSeparator + 1)), delta);
}
void SearchEngineWrapper::SearchEngineWrapper::Find(System::String^ file) {	
	const array<const char*, 2> *res = searchEngine->find(ConvertToChar(file));
	searchResults->Clear();
	for (int i = 0; res[i][0]; i++)
	{
		SearchResult^ tmp = gcnew SearchResult;
		tmp->path = gcnew String(res[i][0]);
		tmp->name = gcnew String(res[i][1]);
		searchResults->Add(tmp);
	}
}

//private functions:
char* SearchEngineWrapper::SearchEngineWrapper::ConvertToChar(System::String^ text) {
	char *cNow = new char[text->Length];
	sprintf(cNow, "%s", text);
	return cNow;
}