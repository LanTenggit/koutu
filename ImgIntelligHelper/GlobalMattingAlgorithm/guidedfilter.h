#include <opencv2/opencv.hpp>

using namespace cv;

class GuidedFilterImpl;

class GuidedFilter
{
public:
	GuidedFilter(const Mat &I, int r, double eps);
	~GuidedFilter();

	Mat filter(const Mat &p, int depth = -1) const;

private:
	GuidedFilterImpl * impl_;
};

Mat guidedFilter(const Mat &I, const Mat &p, int r, double eps, int depth = -1);

