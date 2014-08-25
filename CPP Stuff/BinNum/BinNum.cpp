// BinNum.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <iostream>

using namespace std;

class BigNum
{
	vector<__int8> digits;

public:
	BigNum(int seed)
	{
		while (seed != 0)
		{
			digits.push_back(seed % 10);
			seed /= 10;
		}
	}

	BigNum& operator+(const BigNum& additive)
	{
		int carry = 0;
		unsigned int i = 0;
		for (i = __max(additive.digits.size(), digits.size()) - 1; i >= 0; i--)
		{
			int operand1 = (i < digits.size() - 1) ? digits[i] : 0;
			int operand2 = (i < additive.digits.size() - 1) ? additive.digits[i] : 0;
			int sum = operand1 + operand2 + carry;
			//digits.emplace(i, sum % 10);
			carry = sum / 10;
		}
		return *this;
	}

	BigNum& operator+(int additive)
	{
		int carry = additive;
		auto digitIt = digits.begin();
		for (; digitIt < digits.end() && carry != 0; digitIt++)
		{
			int sum = (*digitIt) + carry;
			(*digitIt) = sum % 10;
			carry = sum / 10;
		}
		if (digitIt == digits.end() && carry != 0)
		{
			digits.push_back(carry);
		}
		return *this;
	}

	void Display()
	{
		auto digitIt = digits.rbegin();
		for (; digitIt < digits.rend(); digitIt++)
		{
			cout << (int)(*digitIt);
		}
		cout << endl;
	}

};

int _tmain(int argc, _TCHAR* argv[])
{
	BigNum hund25(125);
	hund25.Display();

	BigNum foo = hund25 + BigNum(225);
	foo.Display();

	return 0;
}

