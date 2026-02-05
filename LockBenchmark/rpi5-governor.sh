cat /sys/devices/system/cpu/cpu0/cpufreq/scaling_min_freq
cat /sys/devices/system/cpu/cpu0/cpufreq/scaling_max_freq

1500000
echo 2400000 > /sys/devices/system/cpu/cpu0/cpufreq/scaling_max_freq
echo 2400000 > /sys/devices/system/cpu/cpu0/cpufreq/scaling_min_freq
