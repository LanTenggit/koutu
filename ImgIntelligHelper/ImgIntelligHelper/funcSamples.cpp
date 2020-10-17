#include "stdafx.h"
#include "funcSamples.h"
#include "AlphaMatting.h"

// 测试C++打包dll供C#调用写的测试函数
double Add(double a, double b)
{
	return a + b;
}

// 测试C++打包dll供C#调用写的测试函数
double Sub(double a, double b)
{
	return a - b;
}

// 测试C++打包dll供C#调用写的测试函数
double Mul(double a, double b)
{
	return a * b;
}

// 测试C++打包dll供C#调用写的测试函数
double Div(double a, double b)
{
	return a / b;
}


// 原想将结果包装成Class或Struct对象返回给c#调用端，调试失败
void GetMatteData(char* sInput, char* sTrimap, struct AlphaData aData)
{
	AlphaMatting alphaMatHelper;
	alphaMatHelper.loadImage(sInput);
	alphaMatHelper.loadTrimap(sTrimap);
	alphaMatHelper.solveAlpha();

	char sOut[] = "E:/Workspace/WPF/Demos/OpenCv/OpenCvCollections/MattingImgs/trimap6666.jpg";
	alphaMatHelper.save(sOut);
	aData.Map = alphaMatHelper.alpha;
}

// 原想直接将结果直接以byte[]形式返回给c#调用端，不熟悉C++、C#的数据互传交互，调试失败
UCHAR* GetAlphaData(char* sInput, char* sTrimap, UCHAR* buf)
{
	AlphaMatting alphaMatHelper;
	alphaMatHelper.loadImage(sInput);
	alphaMatHelper.loadTrimap(sTrimap);
	alphaMatHelper.solveAlpha();

	char sOut[] = "E:/Workspace/WPF/Demos/OpenCv/OpenCvCollections/MattingImgs/trimap6666.jpg";
	alphaMatHelper.save(sOut);

	//alphaMatHelper.getMatte();
	buf = (UCHAR*)alphaMatHelper.GetMatteData();
	return (UCHAR*)alphaMatHelper.GetMatteData();
}

// 原想直接将alpha数组在C#中以Ref 或者 Out 参数形式传出，调试失败
int* GetAlphaMap(char* sInput, char* sTrimap, int* data)
{
	AlphaMatting alphaMatHelper;
	alphaMatHelper.loadImage(sInput);
	alphaMatHelper.loadTrimap(sTrimap);
	alphaMatHelper.solveAlpha();

	char sOut[] = "E:/Workspace/WPF/Demos/OpenCv/OpenCvCollections/MattingImgs/trimap6666.jpg";
	alphaMatHelper.save(sOut);

	alphaMatHelper.GetAlphaMap();
	data = alphaMatHelper.Map;

	return alphaMatHelper.Map;
}

// 最终Success方式
int* GetMatteMap(char* sInput, char* sTrimap)
{
	AlphaMatting alphaMatHelper;
	alphaMatHelper.loadImage(sInput);
	alphaMatHelper.loadTrimap(sTrimap);
	alphaMatHelper.solveAlpha();

	//char sOut[] = "E:/Workspace/WPF/Demos/OpenCv/OpenCvCollections/MattingImgs/trimap6666.jpg";
	//alphaMatHelper.save(sOut);

	alphaMatHelper.GetAlphaMap();
	return alphaMatHelper.Map;
}