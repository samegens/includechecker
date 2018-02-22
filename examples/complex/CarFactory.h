#ifndef __CARFACTORY_H__
#define __CARFACTORY_H__

#include "typedefs.h"

DECLARE_CLASS(Car)

class CarFactory
{
public:
	enum ECarType
	{
		CT_HYBRID,
		CT_SUPER
	};

	static pCar					CreateCar(ECarType inCarType);
};

#endif // __CARFACTORY_H__
