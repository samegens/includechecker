#ifndef __HYBRIDCAR_H__
#define __HYBRIDCAR_H__

#include "typedefs.h"
#include "Car.h"


DECLARE_CLASS(HybridCar)

class HybridCar : public Car
{
public:
	void						Charge();
};

#endif // __HYBRIDCAR_H__
