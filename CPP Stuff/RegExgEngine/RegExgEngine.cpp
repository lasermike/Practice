// RegExgEngine.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <string>
#include <iostream>

using namespace std;

string GetNextPattern(const string& re, int start)
{
	for (unsigned int i = start; i < re.length(); i++)
	{
		if (re[i] == '*')
		{
			string newOff = re.substr(start, i - start + (i == start ? 1 : 0));
			return newOff;
		}
	}
	return re.substr(start);
}

// Return length of match
int FindMatch(const string& pattern, const string& s, int start, bool wild)
{
	if (pattern.length() > s.length() - 1)
	{
		return wild;
	}

	for (unsigned int i = 0; i < pattern.length(); i++)
	{
		if (s[i + start] == pattern[i])
		{
			wild = false;
		}
		else if (!wild)
		{
			if (i < pattern.length() - 1)
			{
				return 0; // Didn't get to the end of the pattern to match
			}
			return i; // Matched
		}

	}

	//if ()
	
	return pattern.length();
}

bool EvaluateRegExp(string re, string s)
{
	bool match = false;
	int startNextPattern = 0;
	int startNextS = 0;
	for (unsigned int i = 0; i + startNextPattern < re.length(); i++)
	{
		bool wild = false;
		string pattern;
		pattern = GetNextPattern(re, startNextPattern);
		cout << "Got pattern: " << pattern << endl;
		if (pattern[0] == '*')
		{
			pattern = GetNextPattern(re, ++startNextPattern);
			cout << "Got pattern: " << pattern << endl;
			wild = true;
		}
		
		int matchLen = FindMatch(pattern, s, startNextS, wild);
		cout << "match: " << matchLen << endl;
		if (matchLen == 0)
			return false;
		startNextPattern += pattern.length();
		startNextS += matchLen;
		match = true;
	}

	return match;
}


int _tmain(int argc, _TCHAR* argv[])
{
	cout << "Evaled:" << EvaluateRegExp("da*cd", "dabc") << endl;
	return 0;
}

