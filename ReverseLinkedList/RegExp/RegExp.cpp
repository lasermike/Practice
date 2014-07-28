// RegExp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <string>
#include <iostream>
#include <queue>

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
	for (unsigned int i = 0; i < pattern.length(); i++)
	{
		if (s[i + start] == pattern[i] || pattern[i] == '.')
		{
			continue;
		}
		else
		{
			return (i == pattern.length() - 1);
		}
	}
	return 
}

bool RunRegExp(const string re, const string s)
{
	cout << "RunRegExp(" << re << ", " << s << ")" << endl;
	queue<string> patterns;
	unsigned int startNextPattern = 0;

	while (startNextPattern < re.length())
	{
		string pattern;
		pattern = GetNextPattern(re, startNextPattern);
		cout << "Got pattern: " << pattern << endl;
		patterns.push(pattern);
		startNextPattern += pattern.length();
	}

	int startNextS = 0;
	while (!patterns.empty())
	{
		bool wild = false;
		string pattern = patterns.front();
		patterns.pop();

		if (pattern[0] == '*')
		{
			if (patterns.empty())
				return true;

			pattern = patterns.front();
			patterns.pop();

			wild = true;
		}
		cout << "Current pattern: " << pattern << "\t" << wild << endl;

		int matchLen = FindMatch(pattern, s, startNextS, wild);
		
		if (!matchLen)
			return false;

		startNextS += matchLen;
	}

	return true;
}

int _tmain(int argc, _TCHAR* argv[])
{
	cout << "RESULT: " << RunRegExp(string("da*cd"), string("dabc")) << endl;

	return 0;
}

