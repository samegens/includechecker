#ifndef __PRAGMAS_H__
#define __PRAGMAS_H__

// This is an example of a file that doesn't really add any tags, it's only used to change the way that the compiler behaves.
// IncludeChecker will always mark the use of such a file as 'unused', so we have to tell it to always ignore this file,
// see includechecker-complex.xml.

// Example pragma: disable warning "nonstandard extension used : nameless struct/union", not applicable in this demo application.
#pragma warning(disable: 4201)

#endif // __PRAGMAS_H__
