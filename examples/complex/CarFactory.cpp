#include "pragmas.h"
#include "CarFactory.h"
#include "Cars.h"			// This include doesn't declare anything on its own, so we have to mark it as an interface header, see includechecker-complex.xml.
#include <stdio.h>


pCar CarFactory::CreateCar(ECarType inCarType)
{
	switch (inCarType)
	{
		case CT_HYBRID:
			printf("Creating hybrid car.\n");
			return new HybridCar();
		case CT_SUPER:
			printf("Creating super car.\n");
			return new SuperCar();
	}

	return NULL;
}
