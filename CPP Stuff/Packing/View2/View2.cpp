// View2.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "View2.h"
#include <time.h>

// Rect drawing
#include <objidl.h>
#include <gdiplus.h>
#pragma comment (lib,"Gdiplus.lib")

#include "..\Packing\Packing.h"

#define MAX_LOADSTRING 100
const unsigned int WM_CREATERECTS = WM_USER + 1;
const unsigned int windowSizeMargin = 40;
const int drawMargin = 16;


// Global Variables:
HINSTANCE hInst;								// current instance
TCHAR szTitle[MAX_LOADSTRING];					// The title bar text
TCHAR szWindowClass[MAX_LOADSTRING];			// the main window class name
Packer* _packer = NULL;

void TestCase3()
{
	_packer = new Packer(100, 100);
	list<Rect> allocated;

	PackerTest::GetRect(40, 20, _packer, allocated);
	_packer->OutputFree();
	_packer->CheckFree(Rect(0, 21, 100, 100));
	_packer->CheckFree(Rect(41, 0, 100, 20));

	PackerTest::GetRect(50, 80, _packer, allocated);
	_packer->OutputFree();
	_packer->CheckFree(Rect(0, 30, 40, 80));
	_packer->CheckFree(Rect(0, 90, 100, 100));
}

void TestCase4()
{
	srand(0);

	_packer = new Packer(256, 256);
	list<Rect> allocated;

	for (int i = 0; i < 4; i++)
	{
		int height = (int) ( (rand() / (float) RAND_MAX) * 256);
		int width = (height * 4) / 3;
		PackerTest::GetRect(width, height, _packer, allocated);
		_packer->OutputFree();
	}

}

VOID OnPaint(HDC hdc)
{
	if (_packer != NULL)
	{
		Gdiplus::Graphics graphics(hdc);

		// Draw atlas size
		Gdiplus::Pen blackPen(Gdiplus::Color(255, 0, 0, 0));
		graphics.DrawRectangle(&blackPen, drawMargin, drawMargin, _packer->GetWidth(), _packer->GetHeight());

		// Draw allocated
		//Gdiplus::Pen greenPen(Gdiplus::Color(255, 0, 255, 0));
		Gdiplus::SolidBrush greenBrush(Gdiplus::Color(128, 0, 255, 0));
		for (auto i = _packer->GetAllocatedList().begin(); i != _packer->GetAllocatedList().end(); i++)
		{
			graphics.FillRectangle(&greenBrush, i->x1 + drawMargin, i->y1 + drawMargin,
								   i->Width(), i->Height());
		}

		// Draw allocated
		//Gdiplus::Pen bluePen(Gdiplus::Color(255, 0, 0, 255));
		Gdiplus::SolidBrush blueBrush(Gdiplus::Color(128, 0, 0, 255));
		for (auto i = _packer->GetFreeList().begin(); i != _packer->GetFreeList().end(); i++)
		{
			graphics.FillRectangle(&blueBrush, i->x1 + drawMargin, i->y1 + drawMargin,
				i->Width(), i->Height());
		}

	}
}

/************ Window management ****************/

// Forward declarations of functions included in this code module:
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	// Initialize GDI+.
	Gdiplus::GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR           gdiplusToken;
	Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	MSG msg;
	HACCEL hAccelTable;

	// Initialize global strings
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_VIEW2, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	// Perform application initialization:
	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_VIEW2));

	// Main message loop:
	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	Gdiplus::GdiplusShutdown(gdiplusToken);

	return (int) msg.wParam;
}



//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_VIEW2));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
	wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_VIEW2);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassEx(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//
//        In this function, we save the instance handle in a global variable and
//        create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance; // Store instance handle in our global variable

   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   BOOL consoleShown = AllocConsole();
   freopen("CONOUT$", "w", stdout);

   return TRUE;
}

void SetWindowSize(HWND hWnd)
{
	RECT rect, clientRect;

	if (GetWindowRect(hWnd, &rect))
	{
		GetClientRect(hWnd, &clientRect);
		int xfudge = (rect.right - rect.left) - (clientRect.right - clientRect.left);
		int yfudge = (rect.bottom - rect.top) - (clientRect.bottom - clientRect.top);

		SetWindowPos(hWnd, NULL, rect.left, rect.top, _packer->GetWidth() + windowSizeMargin + xfudge, _packer->GetHeight() + windowSizeMargin + yfudge, 0);
	}
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_COMMAND	- process the application menu
//  WM_PAINT	- Paint the main window
//  WM_DESTROY	- post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	int wmId, wmEvent;
	PAINTSTRUCT ps;
	HDC hdc;
	BOOL ret;
	
	switch (message)
	{
	case WM_CREATE:
		PostMessage(hWnd, WM_CREATERECTS, 0, 0);
		break;
	case WM_CREATERECTS:
		TestCase4();
		ret = RedrawWindow(hWnd, NULL, NULL, RDW_ERASE | RDW_INTERNALPAINT);
		SetWindowSize(hWnd);
		break;
	case WM_COMMAND:
		wmId    = LOWORD(wParam);
		wmEvent = HIWORD(wParam);
		// Parse the menu selections:
		switch (wmId)
		{
		case IDM_ABOUT:
			DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
			break;
		case IDM_EXIT:
			DestroyWindow(hWnd);
			break;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
		break;
	case WM_PAINT:
		hdc = BeginPaint(hWnd, &ps);

		OnPaint(hdc);

		EndPaint(hWnd, &ps);
		break;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, LOWORD(wParam));
			return (INT_PTR)TRUE;
		}
		break;
	}
	return (INT_PTR)FALSE;
}
