// Thread1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <string>
#include <thread>
#include <iostream>
#include <mutex>

std::mutex num_mutex;
int num = 0;

void printNum()
{
	bool cont = true;
	while (cont)
	{
		std::lock_guard<std::mutex> lock(num_mutex);
		std::cout << std::this_thread::get_id() << "\t = " << (num++) << "\r\n";

		if (num > 20)
			cont = false;
	}
}


int _tmain(int argc, _TCHAR* argv[])
{
	std::thread t1(printNum);
	std::thread t2(printNum);

	t1.join();
	t2.join();

	return 0;
}

