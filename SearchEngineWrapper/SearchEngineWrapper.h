// SearchEngineWrapper.h

#pragma once

#include "..\SearchEngine\SearchAlgorithm\search-engine.cpp"
using namespace System;
using namespace System::Collections::Generic;

namespace SearchEngineWrapper {

	public ref struct SearchResult {
		String^ name;
		String^ path;
	};

	public ref class SearchEngineWrapper
	{
	public:

		SearchEngineWrapper(System::String ^ indexFile);

		void Save(System::String^ filename);

		void Remove(System::String ^ path);

		void Insert(System::String^ filename, int priority);

		void AddPriority(System::String ^ path, int delta);

		void Find(System::String^ file);

		List<SearchResult^>^ searchResults;

	private:
		SearchEngine *searchEngine;

		char * ConvertToChar(System::String^ text);
	};
}
