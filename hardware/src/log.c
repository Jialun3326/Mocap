#include "log.h"

#include <zephyr/sys/printk.h>
#include <stdlib.h>

void error(uint8_t log_level, const char* format, ...) {
    if (log_level > LOG_LEVEL) {
        return;
    }
    va_list args;
    va_start(args, format);
    printk(RED "[ERROR] " RESET);
    vprintk(format, args);
    printk("\n\r");
    va_end(args);
    exit(1);
}

void warn(uint8_t log_level, const char* format, ...) {
    if (log_level > LOG_LEVEL) {
        return;
    }
    va_list args;
    va_start(args, format);
    printk(YELLOW "[WARN]  " RESET);
    vprintk(format, args);
    printk("\n\r");
    va_end(args);
}

void info(uint8_t log_level, const char* format, ...) {
    if (log_level > LOG_LEVEL) {
        return;
    }
    va_list args;
    va_start(args, format);
    printk(BLUE "[INFO]  " RESET);
    vprintk(format, args);
    printk("\n\r");
    va_end(args);
}