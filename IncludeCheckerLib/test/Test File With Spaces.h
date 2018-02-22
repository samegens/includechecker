
// defines
#define JUST_DEFINE
#define DEFINE_VALUE blah
#define DEFINE_FUNCTION(x) blah##x
#define DEFINE_MULTILINE_FUNCTION(x,y) {\
  x;\
  y;\
}

// typedef
typedef int IntTypedef;

// global functions
void VoidFunc();
int IntFunc(int inParam);
extern int ExternFunc(int inParam);

// enum
enum GlobalEnum
{
	EnumValue1,
	EnumValue2
};

// variables
int GlobalVar;
extern int ExternVar;
volatile int VolatileVar;

// namespace
namespace GlobalNamespace
{
	void VoidFuncInNamespace();
	#define DEFINE_IN_NAMESPACE
}

// class
class GlobalClass
{
	#define DEFINE_IN_CLASS
public:
	// constructor
    GlobalClass();
    // destructor
    ~GlobalClass();
    
    // function
    void ClassFunc();
    
    // enum
    enum ClassEnum
    {
	    ClassEnumValue1,
	    ClassEnumValue2
	};

	// variables   
	int mClassVariable;
	static int mStaticClassVariable;
};

