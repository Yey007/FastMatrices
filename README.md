# FastMatrices
Making matrix operations easy and fast since 2020

Hello! This is a library written in C# (.NET Framework) for easily managing matrices
and operating on them quickly. It uses the ILGPU library for GPU acceleration.

Note: If you're using only numbers in your matrix (like most sane people) it will be easier for you to use the FastMatrices.Simple library.
https://github.com/Yey007/FastMatrices.Simple

# Getting started
If you want to start using this library, I would suggest you look in the FastMatrixOperations.Samples
folder first. It contains samples on all the topics you will need to get started. If you're not sure about something, 
don't be afraid to ask! Open an issue here on the GitHub and I'll try to help you out.

# Features and proper usage
This library contains more features than the FastMatrices.Simple library in exchange for a little ease of use.
Like the simple library, there are three different "operators". The single-threaded operator, the multi-threaded operator,
and the GPU operator. Each excel at different things, so listen closely and don't just jump to the GPU operator because 
you think it's the fastest. If you use the wrong operator in the wrong place, it *will* cost you performance.

**1. The single-threaded operator**

This is likely the operator you will be using the most. It runs on a single thead and has no overhead in terms of 
initialization or threads because it all happens on the main thread. It supports any type you would want to work on 
as long as it implements the `IOperatable<T>` interface. Wrappers for basic number types are provided.
  
**2. The multi-threaded operator**

You will probably seldom use this operator but it has its niche right in the middle of the GPU and single-threaded operators.
This is because it has a good amount of overhead in allocating jobs to different threads. Like the single-threaded operator, 
it supports all types as long as it implemenets `IOperatable<T>`.

**3. The GPU operator**

The GPU operator allows you to invoke the might of your graphics card to multiply matrices very quickly. Sounds great,
but it has it's limitations. First of all, initializing this operator is *very, very* slow. It takes around 1.5 *seconds*.
Unfortunately, there isn't really a way around that, but it is a one time cost. If you know you're going to be using it, initialize it
right at the start of your program. The second problem is copying. Matrices must be copied to the GPU in order to be used
on the GPU. This usually takes around 10 milliseconds but depends on the size of your matrix. When you operate on a matrix,
the operator will handle copying for you. However, if you would like, you may take copying into your hands with `CopyToGPU()`.
Copying during downtime of your program can give you better results. Finally, the last limitation is in types. The GPU operator
can only opperate on buffered matrices. This means you are limited to types which implement `IGPUOperatable<T>`, which limits you to
structs and primitives only. Currently, there is no way around this. 

# Contributing
If you want to contribute to this, there aren't many guidelines really. If you want to add a feature, just open an issue,
fork the repo, and start working. Open a PR when you want to. Just try to stay clean with your code, write some
tests/update the existing ones when you're done if necessary, and don't go over 100 characters per line. If you're ever confused by
some shitty code I wrote, don't ask because I probably don't know either (Jk you can ask any time). Thanks for reading!
