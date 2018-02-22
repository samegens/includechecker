#ifndef __CAR_H__
#define __CAR_H__

#include "typedefs.h"

// We need the following include, because the declaration of WheelQuad needs to know the size.
// However IncludeChecker will not find any tags from Wheel.h so we have to tell it to look for types with a suffix ("Quad"),
// see includechecker-complex.xml.
#include "Wheel.h"


DECLARE_CLASS(Car)

class Car
{
public:
								Car();

	void						Drive();

private:
	WheelQuad					mWheels;
};

#endif // __CAR_H__
