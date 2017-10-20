#change this to the name of the Main class file, without file extension
MAIN_FILE = PshGP

#change this to the depth of the project folders
#if needed, add a preffix for a common project folder
CSHARP_SOURCE_FILES = $(wildcard */*/*.cs */*.cs *.cs)

UNITY_HOME=/Applications/Unity5.3.2f1
#add needed flags to the compilerCSHARP_FLAGS = -out:$(EXECUTABLE)
CSHARP_FLAGS = -unsafe -r:ICSharpCode.SharpZipLib.dll -debug
# CSHARP_FLAGS = -r:$(UNITY_HOME)/Unity.app/Contents/Frameworks/Mono/lib/mono/micro/mscorlib.dll

#change to the environment compiler
CSHARP_COMPILER = mcs
# CSHARP_COMPILER = $(UNITY_HOME)/Unity.app/Contents/Frameworks/Mono/bin/mcs

#if needed, change the executable file
EXECUTABLE = $(MAIN_FILE).exe
LIBRARY = $(MAIN_FILE).dll

#if needed, change the remove command according to your system
RM_CMD = -rm -f $(EXECUTABLE)



all: $(LIBRARY) $(EXECUTABLE)

$(EXECUTABLE): $(CSHARP_SOURCE_FILES)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) $(CSHARP_SOURCE_FILES) -out:$(EXECUTABLE) -main:$(MAIN_FILE)

# $(CSHARP_COMPILER) $(CSHARP_FLAGS) -r:$(LIBRARY) $(MAIN_FILE).cs -out:$(EXECUTABLE)

$(LIBRARY): $(CSHARP_SOURCE_FILES)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -target:library $(CSHARP_SOURCE_FILES) -out:$(LIBRARY)

run: all
	./$(EXECUTABLE)

clean:
	$(RM_CMD)
