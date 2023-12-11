#include "LSM6DSM.h"

status_t initialise_imu(configuration_t accelerometer, configuration_t gyroscope) {
    // IMU.all_ones_counter = 0;
    // IMU.non_success_counter = 0;
    IMU.gyroscope = gyroscope;
    IMU.accelerometer = accelerometer;

    if (!i2c_is_ready_dt(&IMU.i2c)) {
        error(2, "I2C is not ready");
        return HARDWARE_ERROR;
    }

    // Wait for a few ms
    // FIXME: Is this enough time? Is this even necessary?
    k_msleep(10);

    // Check the ID register for a successful initialization
    uint8_t id;
    status = read_byte(WHO_AM_I, &id);
    if (status != SUCCESS) {
        error(2, "Failed to read ID");
        return status;
    }
    if (id != WHO_AM_I_REG_VALUE) {
        error(2, "ID is not correct");
        return HARDWARE_ERROR;
    }

    // Setup accelerometer
    uint8_t data_to_write = 0;
    switch (IMU.accelerometer.bandwidth) {
        default:   IMU.accelerometer.bandwidth = 400;
        case 400:  data_to_write |= BW0_XL_400Hz;     break;
        case 1500: data_to_write |= BW0_XL_1500Hz;    break;
    }
    switch (IMU.accelerometer.range) {
        default: IMU.accelerometer.range = 2;
        case 2:  data_to_write |= FS_XL_2g;   break;
        case 4:  data_to_write |= FS_XL_4g;   break;
        case 8:  data_to_write |= FS_XL_8g;   break;
        case 16: data_to_write |= FS_XL_16g;  break;
    }
    switch (IMU.accelerometer.sample_rate) {
        default:   IMU.accelerometer.sample_rate = 104;
        case 1:    data_to_write |= ODR_XL_1_6Hz;       break;
        case 13:   data_to_write |= ODR_XL_12_5Hz;      break;
        case 26:   data_to_write |= ODR_XL_26Hz;        break;
        case 52:   data_to_write |= ODR_XL_52Hz;        break;
        case 104:  data_to_write |= ODR_XL_104Hz;       break;
        case 208:  data_to_write |= ODR_XL_208Hz;       break;
        case 416:  data_to_write |= ODR_XL_416Hz;       break;
        case 833:  data_to_write |= ODR_XL_833Hz;       break;
        case 1660: data_to_write |= ODR_XL_1660Hz;      break;
        case 3330: data_to_write |= ODR_XL_3330Hz;      break;
        case 6660: data_to_write |= ODR_XL_6660Hz;      break;
    }
    status = write_byte(CTRL1_XL, data_to_write);
    if (status != SUCCESS) {
        error(2, "Failed to configure accelerometer");
        return status;
    }
    
    // Configure accelerometer low power mode
    // data_to_write = 0;
    // data_to_write |= XL_HM_MODE_ENABLED;
    // TODO: Check that this works?
    if (IMU.accelerometer.low_power) {
        status = write_byte(CTRL6_C, XL_HM_MODE_ENABLED);
        if (status != SUCCESS) {
            error(2, "Failed to configure accelerometer low power mode");
            return status;
        }
    }

    // Setup gyroscope
    data_to_write = 0;
    switch (IMU.gyroscope.range) {
        default:   IMU.gyroscope.range = 250;
        case 125:  data_to_write |= FS_125_ENABLED; break;
        case 250:  data_to_write |= FS_G_250dps;    break;
        case 500:  data_to_write |= FS_G_500dps;    break;
        case 1000: data_to_write |= FS_G_1000dps;   break;
        case 2000: data_to_write |= FS_G_2000dps;   break;
    }
    switch (IMU.gyroscope.sample_rate) {
        default:   IMU.gyroscope.sample_rate = 104;
        case 13:   data_to_write |= ODR_G_13Hz;     break;
        case 26:   data_to_write |= ODR_G_26Hz;     break;
        case 52:   data_to_write |= ODR_G_52Hz;     break;
        case 104:  data_to_write |= ODR_G_104Hz;    break;
        case 208:  data_to_write |= ODR_G_208Hz;    break;
        case 416:  data_to_write |= ODR_G_416Hz;    break;
        case 833:  data_to_write |= ODR_G_833Hz;    break;
        case 1660: data_to_write |= ODR_G_1660Hz;   break;
        case 3330: data_to_write |= ODR_G_3330Hz;   break;
        case 6660: data_to_write |= ODR_G_6660Hz;   break;
    }
    status = write_byte(CTRL2_G, data_to_write);
    if (status != SUCCESS) {
        error(2, "Failed to configure gyroscope");
        return status;
    }

    // Configure gyroscope low power mode
    // dataToWrite = 0;
    // dataToWrite |= G_HM_MODE_ENABLED;
    // TODO: Check that this works?
    if (IMU.gyroscope.low_power) {
        status = write_byte(CTRL7_G, G_HM_MODE_ENABLED);
        if (status != SUCCESS) {
            error(2, "Failed to configure gyroscope low power mode");
            return status;
        }
    }

    status = write_byte(CTRL8_XL, LOW_PASS_ON_6D_ENABLED);
    if (status != SUCCESS) {
        error(2, "Failed to enable accelerometer low pass filter");
        return status;
    }

    // TODO: Enable low pass on gyro?

    return SUCCESS;
}

status_t read_byte(uint8_t address, uint8_t* data) {
    int return_code = i2c_reg_read_byte_dt(&IMU.i2c, address, data);
    if (return_code != 0) {
        error(5, "Failed to read byte");
        return HARDWARE_ERROR;
    }
    return SUCCESS;
}

status_t read_int16(uint8_t address, int16_t* data) {
    uint8_t buffer[2];
    read_byte(address, &buffer[0]);
    read_byte(address + 1, &buffer[1]);
    // int return_code = i2c_burst_read_dt(&IMU.i2c, address, buffer, 2);
    // if (return_code != 0) {
    //     error(2, "Failed to read int16");
    //     return HARDWARE_ERROR;
    // }
    *data = (buffer[1] << 8) | buffer[0];
    return SUCCESS;
}

status_t write_byte(uint8_t address, uint8_t data) {
    int return_code = i2c_reg_write_byte_dt(&IMU.i2c, address, data);
    if (return_code != 0) {
        error(5, "Failed to write byte");
        return HARDWARE_ERROR;
    }
    return SUCCESS;
}

status_t read_raw_data(uint8_t address, int16_t* data) {
    status = read_int16(address, data);
    if (status != SUCCESS) {
        if (status == ALL_ONES_WARNING) {
            warn(4, "All ones detected");
            // IMU.all_ones_counter++;
        } else {
            warn(4, "Non success detected");
            // IMU.non_success_counter++;
        }
    }
    return status;
}

status_t convert_raw_data(sensor_t sensor, int16_t* data) {
    switch (sensor) {
        case ACCELEROMETER: *data *= (IMU.accelerometer.range / 2); break;
        case GYROSCOPE:     *data *= (IMU.gyroscope.range / 125); break;
        case TEMPERATURE:   *data /= 256; break;
        default:
            error(4, "Sensor not supported");
            return NOT_SUPPORTED;
    }
    return SUCCESS;
}

status_t read_data(sensor_t sensor, uint8_t address, int16_t* data) {
    status = read_raw_data(address, data);
    if (status != SUCCESS) {
        error(3, "Failed to read raw data");
        return status;
    }
    status = convert_raw_data(sensor, data);
    if (status != SUCCESS) {
        error(3, "Failed to convert raw data");
        return status;
    }
    return SUCCESS;
}

status_t read_accelerometer(vector_t* data) {
    status = read_data(ACCELEROMETER, OUTX_L_XL, &data->x);
    if (status != SUCCESS) {
        error(2, "Failed to read accelerometer x");
        return status;
    }
    status = read_data(ACCELEROMETER, OUTY_L_XL, &data->y);
    if (status != SUCCESS) {
        error(2, "Failed to read accelerometer y");
        return status;
    }
    status = read_data(ACCELEROMETER, OUTZ_L_XL, &data->z);
    if (status != SUCCESS) {
        error(2, "Failed to read accelerometer z");
        return status;
    }
    return SUCCESS;
}

status_t read_gyroscope(vector_t* data) {
    status = read_data(GYROSCOPE, OUTX_L_G, &data->x);
    if (status != SUCCESS) {
        error(2, "Failed to read gyroscope x");
        return status;
    }
    status = read_data(GYROSCOPE, OUTY_L_G, &data->y);
    if (status != SUCCESS) {
        error(2, "Failed to read gyroscope y");
        return status;
    }
    status = read_data(GYROSCOPE, OUTZ_L_G, &data->z);
    if (status != SUCCESS) {
        error(2, "Failed to read gyroscope z");
        return status;
    }
    return SUCCESS;
}

status_t read_temperature(int16_t* data) {
    status = read_data(TEMPERATURE, OUT_TEMP_L, data);
    if (status != SUCCESS) {
        error(2, "Failed to read temperature");
        return status;
    }
    return SUCCESS;
}