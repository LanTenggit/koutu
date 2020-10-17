#pragma once

extern "C" __declspec(dllexport) double Add(double a, double b);

extern "C" __declspec(dllexport) double Sub(double a, double b);

extern "C" __declspec(dllexport) double Mul(double a, double b);

extern "C" __declspec(dllexport) double Div(double a, double b);

//extern "C" __declspec(dllexport) int ** GetAlphaArray(char* sInput, char* sTrimap);

extern "C" __declspec(dllexport) void GetMatteData(char* sInput, char* sTrimap, struct AlphaData aData);

extern "C" __declspec(dllexport) UCHAR* GetAlphaData(char* sInput, char* sTrimap, UCHAR* buf);

extern "C" __declspec(dllexport) int* GetAlphaMap(char* sInput, char* sTrimap, int* data);

extern "C" __declspec(dllexport) int* GetMatteMap(char* sInput, char* sTrimap);



