import os
import zlib
from shutil import copyfile

PACKET_DIR = "C:\\EVE\\bin\\packets_1457914730\\"

for i in os.listdir(PACKET_DIR):
    with open(PACKET_DIR + i, 'rb') as f:
        header = ord(f.read(1))
        if header == 126:
            copyfile(PACKET_DIR + i, PACKET_DIR + i + '.eve')
            continue
    try:
        raw = zlib.decompress(open(PACKET_DIR + i, 'rb').read())
        raw_file = open(PACKET_DIR + i + '.eve', 'wb')
        raw_file.write(raw)
        raw_file.close()
    except zlib.error:
        print "zlib.error", i
    except:
        print "unknown except"