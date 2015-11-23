(H):
Once upon a time we found an error in this interop DLL.
(probably, some two names differing only by case, which violated com import/export rules)
(this would impair loading our COM objects as IDispatch extenders for smth in VS, like property grids)
(because for our DLL with that object .NET would try exporting a TLB and fail on that name conflict in a VS reference)
So MSFT released a fixed version of the DLL, mailed it to us, and granted permission for redisting this DLL.
That's why this DLL is kept separately from all the other AnyVs stuff.