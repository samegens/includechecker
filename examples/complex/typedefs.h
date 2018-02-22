#ifndef __TYPEDEFS_H__
#define __TYPEDEFS_H__

#define DECLARE_CLASS(type) \
	class type; \
	typedef const type	c##type; \
	typedef type&		r##type; \
	typedef const type&	rc##type; \
	typedef type*		p##type; \
	typedef type		type##Quad[4];

#endif // __TYPEDEFS_H__
