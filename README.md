# FastMatrices
Making matrix operations easy and fast since 2020

Hello! This is a library written in C# (.NET Framework) for easily managing matrices
and operating on them quickly. It uses the ILGPU library for GPU acceleration.

# Getting started
If you want to start using this library, I would suggest you look in the FastMatrixOperations.Samples
folder first. It contains samples on all the topics you will need to get started. If you're not sure about something, 
don't be afraid to ask! Open an issue here on the GitHub and I'll try to help you out.

# Features
Alright, enough of the intro, let's talk features. There are three main "operators" you can use
to add, subtract, multiply, and transpose matrices.

1. The single threaded operator, which runs operations using a single thread on the CPU,
2. The multi threaded operator, which runs operations using multiple threads on the CPU,
3. The GPU operator, which runs operations on the GPU.

Which one you should use depends on your situation. The GPU is extremely fast on it's own but has a lot of
overhead in initialization and copying. The parallel operator is sometimes faster than the CPU operator,
but often lags behind due to overhead in lauching threads.

In terms of types, FastMatrices supports primatives by default and even your own structs! Want to use doubles or floats
instead of ints? Cool, you can do that. Have a struct you want to add? Go ahead, you can do that. 
However, there are a few caveats.

1. Classes are not supported on the GPU (including `string`). This is due to limitations of the ILGPU, which doesn't support classes (yet).
2. `decimal` is not supported on the GPU either. I think it uses some runtime stuff under the hood, and is therefore not compatible with ILGPU.
3. To use structs on the GPU, you must define what I call a "type operator" struct as well. You can see an example of this under
FastMatrixOperations.Samples.GPU in Structs.cs. This is not necessary for the CPU, you only need to overload operators.

# Contributing
If you want to contribute to this, there aren't many guidelines really. If you want to add a feature, just open an issue,
fork the repo, and start working. Open a PR when you want to. Just try to stay clean with your code, write some
tests/update the existing ones when you're done, and don't go over 100 characters per line. If you're ever confused, by
some shitty code I wrote, don't ask because I probably don't know either (Jk you can ask any time). Thanks for reading!
