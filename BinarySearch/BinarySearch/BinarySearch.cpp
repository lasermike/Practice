// BinarySearch.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "iostream"

using namespace std;

class Node
{
public:
	Node() : value(0), left(NULL), right(NULL)
	{

	}
	Node(int value) : Node()
	{
		this->value = value;
	}

	//private:
	int value;
	Node* left;
	Node* right;

};


class BinaryTree
{
public:
	BinaryTree& Add(int value)
	{
		Node* node = new Node(value);
		if (!root)
		{
			root = node;
		}
		else
		{
			Add(root, value);
		}
		return *this;
	}

	void Display()
	{
		Display(root);
		cout << endl;
	}

	void Remove(int value)
	{
		Remove(root, value);
	}

private:

	bool Remove(Node* current, int value)
	{
		if (current->value == value)
		{
			if (current->left)
			{
				current->value = current->left->value;
				bool killNode = Remove(current->left, current->left->value);
				if (killNode)
				{
					delete current->left;
					current->left = NULL;
				}

			}
			else if (current->right)
			{
				current->value = current->right->value;
				bool killNode = Remove(current->right, current->right->value);
				if (killNode)
				{
					delete current->right;
					current->right = NULL;
				}
			}
			else
			{
				// Leaf node, must die
				return true;
			}
		}
		else if (current->value > value && current->left)
		{
			return Remove(current->left, value);
		}
		else if (current->value < value && current->right)
		{
			return Remove(current->right, value);
		}

		return false;
	}

	void Display(Node* current)
	{
		if (current->left)
		{
			Display(current->left);
		}
		cout << current->value << " ";
		if (current->right)
		{
			Display(current->right);
		}
	}


	void Add(Node* current, int value)
	{
		if (current->value > value)
		{
			if (!current->left)
			{
				current->left = new Node(value);
			}
			else
			{
				Add(current->left, value);
			}
		}
		else //if (current->value <= value)
		{
			if (!current->right)
			{
				current->right = new Node(value);
			}
			else
			{
				Add(current->right, value);
			}
		}
	}

	Node* root = NULL;
};

int _tmain(int argc, _TCHAR* argv[])
{
	BinaryTree tree;

	tree.Add(1).Add(9).Add(2).Add(4).Add(3).Add(1).Add(8).Add(6).Add(9);
	tree.Display();

	cout << "removing 4" << endl;
	tree.Remove(4);
	tree.Display();
	cout << "removing 9" << endl;
	tree.Remove(9);
	tree.Display();
	cout << "removing 2" << endl;
	tree.Remove(2);
	tree.Display();
	cout << "removing 1" << endl;
	tree.Remove(1);
	tree.Display();
	cout << "removing 1" << endl;
	tree.Remove(1);
	tree.Display();

	return 0;
}

