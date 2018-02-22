
#include "define.h"
#include "enum.h"
#include "enumvalue.h"
#include "typedef.h"
#include "class.h"
#include "namespace.h"
#include "function.h"
#include "externfunction.h"
#include "inlinefunction.h"
#include "variable.h"

int x = DEFINE_VALUE;

MyEnum e;

int y = SomeEnumValue2;

MyTypeDef z;

MyClass a;

using namespace MyNamespace;

void func()
{
	MyFunc(3);
	MyExternFunc(4);
	MyInlineFunc(42);
	gMyVar = 42;
}

