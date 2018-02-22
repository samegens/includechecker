#ifndef __WHEEL_H__
#define __WHEEL_H__

#include "typedefs.h"


DECLARE_CLASS(Wheel)			// This will also declare the WheelQuad type used in Car.h.

class Wheel
{
public:
								Wheel() : mSize(15.0f) { }

private:
	float						mSize;
};

#endif // __WHEEL_H__
