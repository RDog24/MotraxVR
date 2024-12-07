

import asyncio
import signal
from bleak import BleakScanner, BleakClient
from bleak.exc import BleakError
import struct
import time


import socket
import time
import struct
import json
import serial
import numpy as np



QUEST = "192.168.60.111"

UDP_IP = "127.0.0.1"
# UDP_PORT = 5011

MULTICAST_GROUP = "224.1.1.1"
MULTICAST_PORT = 5011

# addr_info = socket.getaddrinfo('', None)
# print(addr_info)
 
sock = socket.socket(socket.AF_INET, # Internet
                     socket.SOCK_DGRAM) # UDP


# Set Time-to-Live (TTL) to allow multicast traffic to cross routers
ttl = struct.pack('b', 1)
sock.setsockopt(socket.IPPROTO_IP, socket.IP_MULTICAST_TTL, ttl)

# sock.setsockopt(socket.SOL_SOCKET,socket.SO_REUSEADDR,1)

# sock.setsockopt(socket.SOL_IP,socket.IP_ADD_MEMBERSHIP,
#                 socket.inet_aton(MULTICAST_GROUP)+socket.inet_aton(if_ip))

angle = [0.0] * 39

def generate(b,j):
    angle[j] = b

data_angle = {
    'angle': angle
}


RIGHT_HAND_ELBOW = 7
RIGHT_HAND_WRIST = 8
RIGHT_HAND_SHOULDER = 13
LEFT_HAND_SHOULDER= 12
RIGHT_LEG_THIGH = 15

LEFT_HAND_ELBOW = 10
LEFT_HAND_WRIST = 9
LEFT_HAND_SHOULDER_STRAIGHT = 14
LEFT_HAND_SHOULDER_LATERAL = 6
LEFT_LEG_THIGH = 5


connected_list = []

#include_list = [RIGHT_HAND_ELBOW,RIGHT_HAND_WRIST,RIGHT_HAND_SHOULDER]
include_list = [RIGHT_HAND_ELBOW,RIGHT_HAND_WRIST,RIGHT_HAND_SHOULDER,LEFT_HAND_SHOULDER,LEFT_HAND_WRIST,LEFT_HAND_ELBOW]
# include_list = [RIGHT_HAND_ELBOW ,
#                 RIGHT_HAND_WRIST ,
#                 RIGHT_HAND_SHOULDER_STRAIGHT ,
#                 RIGHT_HAND_SHOULDER_LATERAL , 
#                 LEFT_HAND_ELBOW,
#                 LEFT_HAND_WRIST ,
#                 LEFT_HAND_SHOULDER_STRAIGHT,
#                 LEFT_HAND_SHOULDER_LATERAL]


except_list = [x for x in range(2, 16) if x not in include_list]
DEVICES_COUNT = len(include_list)



################ELBOW/FOREARM#####################
RIGHT_HAND_ELBOW_PITCH_INDEX = 30 #FEATHER 7
RIGHT_HAND_ELBOW_ROLL_INDEX = 32
RIGHT_HAND_ELBOW_YAW_INDEX = 31


LEFT_HAND_ELBOW_PITCH_INDEX = 21 #FEATHER 10
LEFT_HAND_ELBOW_ROLL_INDEX = 23
LEFT_HAND_ELBOW_YAW_INDEX = 22


###############WRIST/HAND#########################
RIGHT_HAND_WRIST_PITCH_INDEX = 27 #27 #FEATHER 8
RIGHT_HAND_WRIST_ROLL_INDEX = 29
RIGHT_HAND_WRIST_YAW_INDEX = 28

LEFT_HAND_WRIST_PITCH_INDEX = 18 #FEATHER  9 
LEFT_HAND_WRIST_ROLL_INDEX = 20 
LEFT_HAND_WRIST_YAW_INDEX = 19 


###############SHOULDER/ARM###########################
RIGHT_HAND_SHOULDER_ROLL_INDEX = 35
RIGHT_HAND_SHOULDER_PITCH_INDEX = 33 #FEATHER 13
RIGHT_HAND_SHOULDER_YAW_INDEX = 34 #FEATHER 12

LEFT_HAND_SHOULDER_PITCH_INDEX = 24 #FEATHER 14
LEFT_HAND_SHOULDER_ROLL_INDEX = 26
LEFT_HAND_SHOULDER_YAW_INDEX = 25 #FEATHER 6


###############THIGH/LEG UP###################################

RIGHT_LEG_THIGH_PITCH_INDEX   = 15  #FEATHER 15 
RIGHT_LEG_THIGH_ROLL_INDEX    = 16



LEFT_LEG_THIGH_PITCH_INDEX   = 6  #FEATHER 5
LEFT_LEG_THIGH_ROLL_INDEX    = 7

def init_position_bicepcurls():
    print("calling  right generate function")
    generate(65, RIGHT_HAND_WRIST_YAW_INDEX)
    generate(65, RIGHT_HAND_SHOULDER_YAW_INDEX)
    generate(65, RIGHT_HAND_ELBOW_YAW_INDEX)
    generate(65, LEFT_HAND_WRIST_YAW_INDEX)
    generate(65, LEFT_HAND_SHOULDER_YAW_INDEX)
    generate(65, LEFT_HAND_ELBOW_YAW_INDEX)
    # generate(-90, RIGHT_HAND_WRIST_PITCH_INDEX)
    # generate(-90, RIGHT_HAND_SHOULDER_PITCH_INDEX)
    # generate(-90, RIGHT_HAND_ELBOW_PITCH_INDEX)
    # generate(180, RIGHT_HAND_WRIST_ROLL_INDEX)
    # generate(180, RIGHT_HAND_SHOULDER_ROLL_INDEX)
    # generate(180, RIGHT_HAND_ELBOW_ROLL_INDEX)    
 

def pitch_value_translation(pitch_val):
    pitch_data = float(pitch_val)
    scaled_pitch = ((pitch_data + 85) * 180 / 149) - 90 # pitch of right elbow
    return scaled_pitch

def yaw_value_translation(yaw_val):
    yaw_data = float(yaw_val)
    
    if yaw_data < 115:
        return 90  # Keep anything below 115 as 90
    elif yaw_data < 0:
        return 0   # Keep any negative values as 0
    else:
        # Scale 115 to 270 to range from 90 to 0
        scaled_yaw = 90 - ((yaw_data - 115) * 90 / (270 - 115))
        return max(scaled_yaw, 0)  # Ensure the value doesn't go below 0



def update_feather_8_right_wrist(roll_val , pitch_val):
    pitch_data = float(pitch_val)
    pitch_data = pitch_value_translation(pitch_data) # pitch of right elbow
    generate(float(pitch_data), RIGHT_HAND_WRIST_PITCH_INDEX)
    generate(float(-roll_val), RIGHT_HAND_WRIST_ROLL_INDEX)
    print("8 right wrist")


def update_feather_7_right_elbow(roll_val , pitch_val):    
    pitch_data = float(pitch_val)
    pitch_data = pitch_value_translation(pitch_data) # pitch of right elbow
    generate(float(pitch_data), RIGHT_HAND_ELBOW_PITCH_INDEX)
    generate(float(-roll_val), RIGHT_HAND_ELBOW_ROLL_INDEX)
    print("7 right elbow")

def update_feather_13_right_shoulder(roll_val , pitch_val):
    pitch_data = float(pitch_val)
    pitch_data = pitch_value_translation(pitch_data) # pitch of right elbow
    generate(float(pitch_data), RIGHT_HAND_SHOULDER_PITCH_INDEX)
    generate(float(-roll_val), RIGHT_HAND_SHOULDER_ROLL_INDEX)
    print("13 right shoulder")

def update_feather_9_left_wrist(roll_val , pitch_val):  
    pitch_data = float(pitch_val)
    pitch_data = pitch_value_translation(pitch_data) # pitch of right elbow
    generate(float(pitch_data), LEFT_HAND_WRIST_PITCH_INDEX)
    generate(float(roll_val), LEFT_HAND_WRIST_ROLL_INDEX)
    print("9 left wrist")

def update_feather_10_left_elbow(roll_val , pitch_val):
    pitch_data = float(pitch_val)
    pitch_data = pitch_value_translation(pitch_data) # pitch of right elbo
    generate(float(pitch_data), LEFT_HAND_ELBOW_PITCH_INDEX)
    generate(float(roll_val), LEFT_HAND_ELBOW_ROLL_INDEX)
    print("10 left elbow")


def update_feather_12_right_shoulder(roll_val , pitch_val):
    pitch_data = float(pitch_val)
    pitch_data = pitch_value_translation(pitch_data) # pitch of right elbow
    generate(float(pitch_data), LEFT_HAND_SHOULDER_PITCH_INDEX )
    generate(float(roll_val), LEFT_HAND_SHOULDER_ROLL_INDEX)
    print("10 left elbow")



def update_feather_14_left_shoulder_straight(pitch_val , yaw_val):
    global LEFT_HAND_SHOULDER_PITCH_INDEX
    global base_yaw_data
    global yaw_val_init_count 
    global YAW_CAL_COUNT 
    
    pitch_data = float(pitch_val)
    pitch_data = ((pitch_data + 80) / 160 ) * 180 # pitch of right elbow
    if pitch_data > 180:
        pitch_data = 180
    if pitch_data < 0:
        pitch_data = 0
    #print("13_pitch ",-pitch_data  )
    print("14")
    generate(float(pitch_data), LEFT_HAND_SHOULDER_PITCH_INDEX)



    '''
    yaw_data = float(yaw_val)

    if ( yaw_val_init_count == YAW_CAL_COUNT ):
        print( "yaw value mod = " , float(yaw_data - base_yaw_data))
        generate(float(yaw_data - base_yaw_data), RIGHT_HAND_SHOULDER_YAW_INDEX)
    else:
        yaw_val_init_count = yaw_val_init_count + 1
        base_yaw_data = base_yaw_data + (yaw_data /YAW_CAL_COUNT)
    
    '''


def update_feather_6_left_shoulder_lateral(pitch_val , yaw_val):
    global LEFT_HAND_SHOULDER_YAW_INDEX
    global base_yaw_data
    global yaw_val_init_count 
    global YAW_CAL_COUNT 
    
    pitch_data = float(pitch_val)
    pitch_data = ((pitch_data + 80) / 160 ) * 180 # pitch of right elbow
    if pitch_data > 180:
        pitch_data = 180
    if pitch_data < 0:
        pitch_data = 0
    #print("13_pitch ",-pitch_data  )
    print("14")
    generate(float(pitch_data), LEFT_HAND_SHOULDER_YAW_INDEX)



    '''
    yaw_data = float(yaw_val)

    if ( yaw_val_init_count == YAW_CAL_COUNT ):
        print( "yaw value mod = " , float(yaw_data - base_yaw_data))
        generate(float(yaw_data - base_yaw_data), RIGHT_HAND_SHOULDER_YAW_INDEX)
    else:
        yaw_val_init_count = yaw_val_init_count + 1
        base_yaw_data = base_yaw_data + (yaw_data /YAW_CAL_COUNT)
    
    '''
    



def update_feather_15_right_thigh(roll_val , pitch_val):
    global RIGHT_LEG_THIGH_PITCH_INDEX
    global RIGHT_LEG_THIGH_ROLL_INDEX
    
    pitch_data = float(pitch_val)
    pitch_data = ((pitch_data + 80) / 160 ) * 180# pitch of right elbow
    if pitch_data > 180:
        pitch_data = 180
    if pitch_data < 0:
        pitch_data = 0
    #print("7_pitch",-pitch_data  )
    generate(float(pitch_data), RIGHT_LEG_THIGH_PITCH_INDEX)

    roll_data = float(roll_val)

    if roll_data >= 0 and roll_data <= 180:
        roll_data = (180 - roll_data)  # this equation is for right hand it gives 0 to 180 for anti clock wise rotation , use roll_data = +(180 - roll_data) for left hand clock wise 0 to 180
    else:
        roll_data = (-roll_data - 180) # this equation is for right hand it gives 0 to 180 for anti clock wise rotation , use roll_data = +(-roll_data - 180) for left hand clock wise 0 to 180print(" 8_wrist roll ",roll_data , " 8_wrist pitch " , pitch_data  )

    if roll_data > 180:
        roll_data = 180
    if roll_data < 0:
        roll_data = 0

    #print(" 7_elbow roll ",-roll_data , " 7_elbow pitch " , pitch_data  )
    print("15")
    #generate(float(-roll_data), RIGHT_LEG_THIGH_ROLL_INDEX)




def update_feather_5_left_thigh(roll_val , pitch_val):
    global LEFT_LEG_THIGH_PITCH_INDEX
    global LEFT_LEG_THIGH_ROLL_INDEX
    
    pitch_data = float(pitch_val)
    pitch_data = ((pitch_data + 80) / 160 ) * 180# pitch of right elbow
    if pitch_data > 180:
        pitch_data = 180
    if pitch_data < 0:
        pitch_data = 0
    #print("7_pitch",-pitch_data  )
    generate(float(pitch_data), LEFT_LEG_THIGH_PITCH_INDEX)

    roll_data = float(roll_val)

    if roll_data >= 0 and roll_data <= 180:
        roll_data = (180 - roll_data)  # this equation is for right hand it gives 0 to 180 for anti clock wise rotation , use roll_data = +(180 - roll_data) for left hand clock wise 0 to 180
    else:
        roll_data = (-roll_data - 180) # this equation is for right hand it gives 0 to 180 for anti clock wise rotation , use roll_data = +(-roll_data - 180) for left hand clock wise 0 to 180print(" 8_wrist roll ",roll_data , " 8_wrist pitch " , pitch_data  )

    if roll_data > 180:
        roll_data = 180
    if roll_data < 0:
        roll_data = 0

    #print(" 7_elbow roll ",-roll_data , " 7_elbow pitch " , pitch_data  )
    print("5")
    #generate(float(-roll_data), LEFT_LEG_THIGH_ROLL_INDEX)

    
    

NRF_UUID_SERVICE = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E"
NRF_UUID_CHARACTERISTIC = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E"

connected = 0
samples_number = 10000050
file_count = 1
all_have_enough_samples = True





data_dict = {f'Feather {i}': [] for i in range(2, 16) if i not in except_list}
packet_dict = {f'Feather {i}': 0 for i in range(2, 16) if i not in except_list}

devices_addresses = []

allowed_devices = [f"Feather {i}" for i in range(2, 16) if i not in except_list]

MAX_CONNECTION_ATTEMPTS = 3
RETRY_DELAY = 5  # in seconds
DISCOVERY_TIMEOUT = 10  # in seconds
DISCOVERY_REATTEMPTS = 5  # Number of reattempts

clients = []  # Store connected clients for cleanup


async def discover_devices():
    global devices_addresses
    print("Intended devices to be turned on:", allowed_devices)
    
    attempts = 0
    while set([device[1] for device in devices_addresses]) != set(allowed_devices):
        scanner = BleakScanner()
        print(f"Attempt {attempts+1}: Discovering devices...")
        devices = await asyncio.wait_for(scanner.discover(), timeout=DISCOVERY_TIMEOUT)
        discovered_devices = []
        for device in devices:
            if device.name and "Feather" in device.name and device.name in allowed_devices:
                if [device.address, device.name] not in devices_addresses:  # Check for duplicates
                    devices_addresses.append([device.address, device.name])
                    discovered_devices.append(device.name)
                    print(f"Discovered device: {device.name} ({device.address})")
        if not discovered_devices:
            print("No devices discovered.")
        if set([device[1] for device in devices_addresses]) != set(allowed_devices):
            attempts += 1
            if attempts >= DISCOVERY_REATTEMPTS:
                print("Maximum discovery attempts reached. Exiting...")
                break
            print("Below devices not discovered. Retrying in 10 seconds...")
            connnected_set = set([device[1] for device in devices_addresses])
            intended_set = set(allowed_devices)
            print( intended_set.difference(connnected_set))
            await asyncio.sleep(5)


async def wait_for_input(prompt: str = ""):
    loop = asyncio.get_event_loop()
    future = loop.run_in_executor(None, input, prompt)
    return await future


async def read_characteristic(address, name, continue_event):
    global connected
    global clients
    global DEVICES_COUNT
    while not continue_event.is_set():
        reading_complete = asyncio.Event()
        
        def handle_notification(sender: int, data: bytearray):
            nonlocal reading_complete
            global file_count
            global samples_number
            global packet_dict
            global data_dict
            
            if len(data) < 12:
                raise ValueError("Invalid data format: Buffer size is too small.")
            
            roll, pitch, yaw = struct.unpack_from('fff', data, offset=0)

            if( name == "Feather 7"):
                update_feather_7_right_elbow(roll , pitch)

            elif ( name == "Feather 8"):
                update_feather_8_right_wrist(roll , pitch )        

            elif( name == "Feather 13"):
                update_feather_13_right_shoulder( roll,pitch)

            elif( name == "Feather 12"):
                update_feather_12_right_shoulder(roll ,pitch)

            elif( name == "Feather 10"):
                update_feather_10_left_elbow(roll , pitch)

            elif ( name == "Feather 9"):
                update_feather_9_left_wrist(roll , pitch)

            
            
            
            elif( name == "Feather 14"):
                update_feather_14_left_shoulder_straight(pitch , yaw)

            elif( name == "Feather 6"):
                update_feather_6_left_shoulder_lateral(pitch , yaw)

            elif( name == "Feather 15"): 
                update_feather_15_right_thigh(roll ,pitch)

            elif( name == "Feather 5"):
                update_feather_5_left_thigh(roll ,pitch)


                
            #generate(float(pitch), 21)
            data = json.dumps(data_angle)
            #print(data)
            sock.sendto(data.encode("utf-8"), (QUEST, MULTICAST_PORT))

            
            values = [roll, pitch, yaw]
            values.append(time.time())
            #print(f"Data from device {name} ({address}): {values}")
            
            if packet_dict[name] < samples_number:
                data_dict[name].append(values)
                packet_dict[name] = packet_dict[name] + 1

            global all_have_enough_samples
            all_have_enough_samples = True
            for packet in packet_dict.values():
                if packet < samples_number:
                    all_have_enough_samples = False
                    break

            # if all_have_enough_samples:
            #     file_name = "r" + str(file_count) + ".txt"
            #     print(file_name)
            #     with open(file_name, 'w') as f:
            #         for i in range(samples_number):
            #             for device_name, readings in data_dict.items():
            #                 f.write(device_name)
            #                 f.write(",")
            #                 readings_list = readings[i]
            #                 readings_str = ''
            #                 for v in readings_list:
            #                     readings_str += str(v) + ','
            #                 readings_str = readings_str.rstrip(',')
            #                 f.write(readings_str)
            #                 f.write(',')
            #             f.write('\n')
                data_dict = {f'Feather {i}': [] for i in range(2, 16) if i not in except_list}
                packet_dict = {f'Feather {i}': 0 for i in range(2, 16) if i not in except_list}
                file_count += 1
                reading_complete.set()

        attempt = 1
        global connected_list
        while attempt <= MAX_CONNECTION_ATTEMPTS:
            try:
                async with BleakClient(address) as client:
                    clients.append(client)  # Store connected client for cleanup
                    print(f"Connected to device {name} ({address}) on attempt {attempt}")
                    connected += 1
                    while ( connected != DEVICES_COUNT):
                        print("Device count = " , DEVICES_COUNT  , "Total Connected = " , connected)
                        connected_list.append(name)
                        #print(f"connected list {connected_list}") 
                        print(f"NOT CONNECTED LIST IS {list(set(allowed_devices) - set(connected_list))}")
                        await asyncio.sleep(5)
                    print("Device count = " , DEVICES_COUNT  , "Total Connected = " , connected)
                    print("all devices connectted !!")
                    await asyncio.sleep(5)
                    await client.start_notify(NRF_UUID_CHARACTERISTIC, handle_notification)
                    await reading_complete.wait()
                    await client.stop_notify(NRF_UUID_CHARACTERISTIC)
                    break
            except BleakError as e:
                print(f"Failed to connect to {name} ({address}) on attempt {attempt}: {e}")
                attempt += 1
                await asyncio.sleep(RETRY_DELAY)
        
        if attempt > MAX_CONNECTION_ATTEMPTS:
            print(f"Maximum connection attempts reached for device {name} ({address}). Skipping...")
            continue


async def cleanup(signal, loop):
    print("Received exit signal")
    # Disconnect from Bleak clients
    if clients:
        await asyncio.gather(*[client.disconnect() for client in clients], return_exceptions=True)
    loop.stop()


async def main():
    await discover_devices()    
    continue_event = asyncio.Event()
    tasks = [read_characteristic(addr, name, continue_event) for addr, name in devices_addresses if name in allowed_devices]
    await asyncio.gather(*tasks, return_exceptions=True)    
    #await wait_for_input("Press Enter to continue reading data...")
    #continue_event.set()


if __name__ == "__main__":
    init_position_bicepcurls()
    loop = asyncio.get_event_loop()
    signal.signal(signal.SIGINT, lambda s, f: loop.create_task(cleanup(s, loop)))
    loop.run_until_complete(main())
