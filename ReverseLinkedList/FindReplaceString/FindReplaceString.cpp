// FindReplaceString.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "iostream"


bool _TestFirstChars(char* query, char* data)
{
	if (*query == 0 || *data == 0)
	{
		return true;
	}

	// First letter match
	if (*query == *data)
	{
		return _TestFirstChars(query + 1, data + 1);
	}

	return false;

}

int Find(char* query, char* data, bool exact = false)
{
	int dataPos = 0;
	do
	{
		if (*(data + dataPos) == 0)
		{
			break;
		}

		bool val = _TestFirstChars(query, data + dataPos);

		if (val)
		{
			return dataPos;
		}
		dataPos++;
	} while (true);

	return -1;
}

int StringLen(char* data)
{
	int len = 0;
	do
	{
		if (*(data + len) == 0)
		{
			break;
		}
		len++;
	} while (true);

	return len;
}

void InsertChars(char* data, int start, int distance)
{
	int dataStrLen = StringLen(data);
	int len = dataStrLen - start + 1;

	for (int i = len; i >= 0; i--)
	{
		*(data + start + i) = *( (data + start + i) - distance);
		std::cout << " " << data << "\r\n";
	}
	*(data + dataStrLen + distance ) = 0;
}

void RemoveChars(char* data, int start, int distance)
{
	int dataStrLen = StringLen(data);

	for (int i = start; i < dataStrLen; i++)
	{
		*(data + i) = *(data + i + distance);
		std::cout << " " << data << "\r\n";
	}
	*(data + dataStrLen - distance) = 0;
}

void Copy(char* src, char* dest, int len)
{
	for (int i = 0; i < len; i++)
	{
		*(src + i) = *(dest + i);
	}


}

bool Replace(char* query, char* data, char* replaceStr)
{
	int pos = Find(query, data);

	if (pos != -1)
	{
		int queryStrLen = StringLen(query);
		int replaceStrLen = StringLen(replaceStr);

		int delta = replaceStrLen - queryStrLen;

		if (delta > 0)
		{
			InsertChars(data, pos, delta);
		}
		else
		{
			RemoveChars(data, pos + replaceStrLen, abs(delta));
		}

		Copy(data + pos, replaceStr, replaceStrLen);
		
		return true;
	}

	return false;
}

int _tmain(int argc, _TCHAR* argv[])
{
	char query[1000] = "cdef";
	char data[1000] = "abcdefg\0XXXXXXXXX";

	std::cout << "Find: " << Find("c", data) << "\r\n";
	std::cout << (Replace("cd", data, "123") ? data : "not found") << "\r\n\r\n";

	Copy(data, "abcdefg\0XXXXXXXXX", 17);
	std::cout << (Replace("c", data, "123") ? data : "not found") << "\r\n";

	Copy(data, "abcdefg\0XXXXXXXXX", 17);
	std::cout << (Replace("cde", data, "1") ? data : "not found") << "\r\n";

	return 0;
}

