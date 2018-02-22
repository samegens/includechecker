#include "pragmas.h"
// We need this include otherwise the DriveHybridCar function does not compile.
// However IncludeChecker doesn't recognize rHybridCar as usage of one of the tags from HybridCar.h ("HybridCar"),
// so we have to tell it about the "r" prefix, see includechecker-complex.xml.
#include "HybridCar.h"


void DriveHybridCar(rHybridCar ioCar)
{
	ioCar.Drive();
}


int main(int argc, char* argv[])
{
	return 0;
}
