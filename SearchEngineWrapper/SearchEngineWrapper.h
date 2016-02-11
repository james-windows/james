// SearchEngineWrapper.h

#pragma once
#include "..\SearchEngine\shared\SearchAlgorithm\search-engine.cpp"

using namespace System;
using namespace System::Collections::Generic;

namespace SearchEngineWrapper {
	public ref struct SearchResult {
		String^ path;
		int priority;
	};

	public ref class SearchEngineWrapper
	{
	public:

		SearchEngineWrapper(System::String ^ indexFile, int results);

		void Save();

		void Remove(System::String ^ path);

		void RemoveRecursive(System::String ^ path);

		void Insert(System::String^ filename, int priority);

		void AddPriority(System::String ^ path, int delta);

		void Rename(String ^ oldPath, String ^ newPath);

		void Find(System::String^ file);

		char * ConvertToChar(System::String ^ text);

		List<SearchResult^>^ searchResults;

	private:
		String^ indexFile;

		SearchEngine *searchEngine;
	};
}
