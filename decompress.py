import os
import zlib
from shutil import copyfile
import uncompyle2

PACKET_DIR = "C:\\EVE\\bin\\packets_\\"

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
        if ord(raw[0]) == 3:
            uncompyle2.uncompyle_file(PACKET_DIR + i + '.eve', open(PACKET_DIR + i + '.eve.py', 'w'))
    except zlib.error:
        print "zlib.error", i
    except:
        print "unknown except!"
        import sys
        print sys.exc_info()
