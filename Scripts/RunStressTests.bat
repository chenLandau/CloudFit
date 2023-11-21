set /a memory_size_in_GB=%1
:start
tasklist | find "NetworkStressTest.exe" 
if errorlevel 1 (
	echo Network Stress Test is not running, Restarting..
    start "NetworkStressTest" "NetworkStressTest.exe"
)
tasklist | find "CpuStressTest.exe" 
if errorlevel 1 (
	echo Cpu Stress Test is not running, Restarting..
    start "CpuStressTest" "CpuStressTest.exe"
)
tasklist | find "MemoryStressTest.exe" 
if errorlevel 1 (
	echo Memory Stress Test is not running, Restarting..
    start "MemoryStressTest" "MemoryStressTest.exe" %memory_size_in_GB%
)
timeout /t 3 
goto :start

