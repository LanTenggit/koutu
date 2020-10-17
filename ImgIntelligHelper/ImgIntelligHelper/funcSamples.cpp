#include "stdafx.h"
#include "funcSamples.h"
#include "AlphaMatting.h"

// ����C++���dll��C#����д�Ĳ��Ժ���
double Add(double a, double b)
{
	return a + b;
}

// ����C++���dll��C#����д�Ĳ��Ժ���
double Sub(double a, double b)
{
	return a - b;
}

// ����C++���dll��C#����д�Ĳ��Ժ���
double Mul(double a, double b)
{
	return a * b;
}

// ����C++���dll��C#����д�Ĳ��Ժ���
double Div(double a, double b)
{
	return a / b;
}


// ԭ�뽫�����װ��Class��Struct���󷵻ظ�c#���öˣ�����ʧ��
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

// ԭ��ֱ�ӽ����ֱ����byte[]��ʽ���ظ�c#���öˣ�����ϤC++��C#�����ݻ�������������ʧ��
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

// ԭ��ֱ�ӽ�alpha������C#����Ref ���� Out ������ʽ����������ʧ��
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

// ����Success��ʽ
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