import os
from pydub import AudioSegment


yourpath = os.getcwd()
for root, dirs, files in os.walk(yourpath, topdown=False):
    for name in files:
        if os.path.splitext(os.path.join(root, name))[1].lower() == ".mp3":

            directory = root.replace("raw-data", "preprocess-data")
            if not os.path.exists(directory):
                print ("create new folder for %s" % directory)
                os.makedirs(directory)
            outfile = os.path.splitext(os.path.join(directory, name))[0] + ".wav"

            if os.path.isfile(outfile):
                print ("A wav file already exists for %s" % outfile)
            else:
                try:
                    sound = AudioSegment.from_mp3(os.path.join(root, name))
                    sound.export(outfile, format="wav")
                except Exception as e:
                    print (e)
