### Windows X64

| Method (SDK)             | Mean          | Ratio        |
|:-------------------------|--------------:|:-------------|
| **ByObject (10.0.2)**    | **5.1021 ns** | **baseline** |
|   ByInterlocked (10.0.2) |   2.8469 ns   |   +44.20%    |
|   ByLock (10.0.2)        |   3.6404 ns   |   +28.65%    |
|   ByUsingLock (10.0.2)   |   4.5178 ns   |   +11.45%    |
|   ByObject (5.0.17)      |   4.7556 ns   |   +6.79%     |
|   ByInterlocked (5.0.17) |   3.0213 ns   |   +40.78%    |

### Linux X64

| Method (SDK)           | Mean       | Ratio     |
|:-----------------------|-----------:|:----------|
| **ByObject (10.0.2)**  | **7.3407 ns** | **baseline** |
| ByInterlocked (10.0.2) |  6.3244 ns | +13.84%   |
| ByLock (10.0.2)        |  4.1352 ns | +43.67%   |
| ByUsingLock (10.0.2)   |  4.9217 ns | +32.95%   |
| ByObject (5.0.17)      |  6.8308 ns | +6.95%    |
| ByInterlocked (5.0.17) |  6.5285 ns | +11.06%   |

### Windows Arm64

| Method (SDK)           | Mean        | Ratio     |
|:-----------------------|------------:|:----------|
| **ByObject (10.0.2)**  | **19.7561 ns** | **baseline** |
| ByInterlocked (10.0.2) |  14.5324 ns | +26.44%   |
| ByLock (10.0.2)        |  22.0130 ns | -11.42%   |
| ByUsingLock (10.0.2)   |  21.2320 ns | -7.47%    |
| ByObject (5.0.17)      |  31.2339 ns | -58.10%   |
| ByInterlocked (5.0.17) |  23.8464 ns | -20.70%   |

### Linux Arm64

| Method (SDK)           | Mean        | Ratio     |
|:-----------------------|------------:|:----------|
| **ByObject (10.0.2)**  | **30.6923 ns** | **baseline** |
| ByInterlocked (10.0.2) |  20.2718 ns | +33.95%   |
| ByLock (10.0.2)        |  24.6551 ns | +19.67%   |
| ByUsingLock (10.0.2)   |  24.4506 ns | +20.34%   |
| ByObject (5.0.17)      |  32.3971 ns | -5.55%    |
| ByInterlocked (5.0.17) |  21.0709 ns | +31.35%   |
