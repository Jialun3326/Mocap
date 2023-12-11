cmake_minimum_required(VERSION 3.20.0)
find_package(Zephyr REQUIRED HINTS $ENV{ZEPHYR_BASE})

project(i2c_driver)

target_sources(app PRIVATE src/main.c src/LSM6DSM.c src/log.c)
