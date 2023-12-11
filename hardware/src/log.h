#pragma once

#define LOG_LEVEL 1

#define RED     "\x1b[31m"
#define GREEN   "\x1b[32m"
#define YELLOW  "\x1b[33m"
#define BLUE    "\x1b[34m"
#define MAGENTA "\x1b[35m"
#define CYAN    "\x1b[36m"
#define RESET   "\x1b[0m"

void error(uint8_t, const char*, ...);
void warn(uint8_t, const char*, ...);
void info(uint8_t, const char*, ...);