#include "LSM6DSM.h"
#include "log.h"

#include <zephyr/bluetooth/bluetooth.h>
#include <zephyr/bluetooth/gap.h>
#include <zephyr/bluetooth/uuid.h>
#include <zephyr/bluetooth/addr.h>
#include <zephyr/bluetooth/conn.h>

#include <dk_buttons_and_leds.h>


#define DATA_RATE 100 // Hz

#define ID 0

#define DEVICE_NAME CONFIG_BT_DEVICE_NAME
#define DEVICE_NAME_LEN sizeof(DEVICE_NAME) - 1

#define CONNECTION_STATUS_LED DK_LED2


// IMU Constructs

static const configuration_t accelerometer_configuration = {
    .range = 16,         // Max G force readable: 2, 4, 8, 16
    .sample_rate = 6660, // Hz: 1, 13, 26, 52, 104, 208, 416, 833, 1666, 3330, 6660
    .bandwidth = 1500,   // Hz: 400, 1500
    .low_power = false,
};

static const configuration_t gyroscope_configuration = {
    .range = 2000,       // Max deg/s: 125, 250, 500, 1000, 2000
    .sample_rate = 6660, // Hz: 13, 26, 52, 104, 208, 416, 833, 1666, 3330, 6660 // TODO: Experiment with 3330 vs 6660
    .low_power = false,
};


// BLE Constructs

static struct bt_le_adv_param* advertisement_parameters = {
    BT_LE_ADV_PARAM(
        BT_LE_ADV_OPT_CONNECTABLE | BT_LE_ADV_OPT_USE_IDENTITY,
        800, // * 0.625ms = 500ms
        801, // * 0.625ms = 500.625ms
        NULL
    ),
};

typedef struct {
    vector_t accelerometer_data;
    vector_t gyroscope_data;
} imu_data_t;

static imu_data_t imu_data;

static const struct bt_data advertisement_data[] = {
    BT_DATA_BYTES(BT_DATA_FLAGS, (BT_LE_AD_GENERAL | BT_LE_AD_NO_BREDR)),
    BT_DATA(BT_DATA_NAME_COMPLETE, DEVICE_NAME, DEVICE_NAME_LEN),
	BT_DATA(BT_DATA_MANUFACTURER_DATA, (uint8_t*)&imu_data, sizeof(imu_data)),
};

static const struct bt_data scan_response_data[] = {
    BT_DATA_BYTES(BT_DATA_UUID128_ALL, BT_UUID_128_ENCODE(0x00001523, 0x1212, 0xefde, 0x1523, 0x785feabcd123)),
};


void main(void) {
    // TODO: Tidy up this code - package IMU into C++ class?
    
    status = initialise_imu(accelerometer_configuration, gyroscope_configuration);
    if (status != SUCCESS) {
        error(1, "IMU initialisation failed");
    }


    // BLE Setup

    int return_code;

    bt_addr_le_t address;
	return_code = bt_addr_le_from_str("FF:EE:DD:CC:BB:AA", "random", &address);
	if (return_code != 0) {
		error(1, "Invalid BT address (error code: %d)\n", return_code);
	}
    info(1, "Bluetooth address set");

	return_code = bt_id_create(&address, NULL);
	if (return_code < 0) {
		error(1, "Creating new ID failed (error code: %d)\n", return_code);
	}
    info(1, "Bluetooth ID created");

    return_code = bt_enable(NULL);
    if (return_code != 0) {
        error(1, "Bluetooth initialisation failed (error code: %d)", return_code);
    }
    info(1, "Bluetooth initialised");

    return_code = bt_le_adv_start(
        advertisement_parameters,
        advertisement_data, ARRAY_SIZE(advertisement_data),
        scan_response_data, ARRAY_SIZE(scan_response_data)
    );
	if (return_code != 0) {
		error(1, "Advertising failed to start (error code: %d)\n", return_code);
	}
    info(1, "Advertising started");


    vector_t accelerometer_data, gyroscope_data;
    
    while (true) {
        status = read_accelerometer(&accelerometer_data);
        if (status != SUCCESS) {
            warn(1, "Accelerometer read failed");
        }
    
        status = read_gyroscope(&gyroscope_data);
        if (status != SUCCESS) {
            warn(1, "Gyroscope read failed");
        }

        printk(
            // "% d % d % d % d % d % d\n",
            // accelerometer_data.x, accelerometer_data.y, accelerometer_data.z,
            // gyroscope_data.x, gyroscope_data.y, gyroscope_data.z
            "%04x%04x%04x%04x%04x%04x\n",
            (uint16_t)accelerometer_data.x, (uint16_t)accelerometer_data.y, (uint16_t)accelerometer_data.z,
            (uint16_t)gyroscope_data.x, (uint16_t)gyroscope_data.y, (uint16_t)gyroscope_data.z
        );

        // TODO: Send data and ID to host over BLE

        imu_data.accelerometer_data = accelerometer_data;
        imu_data.gyroscope_data = gyroscope_data;

        bt_le_adv_update_data(advertisement_data, ARRAY_SIZE(advertisement_data), scan_response_data, ARRAY_SIZE(scan_response_data));
        
        k_msleep(1000 / DATA_RATE);
    }
}
