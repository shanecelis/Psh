#change this to the name of the Main class file, without file extension
MAIN_FILE = PshGP

#change this to the depth of the project folders
#if needed, add a preffix for a common project folder
CSHARP_SOURCE_FILES = $(wildcard Psh/*/*/*.cs Psh/*/*.cs Psh/*.cs)
CSHARP_TEST_FILES = $(wildcard tests/*/*/*.cs tests/*/*.cs tests/*.cs)

# UNITY_HOME=/Applications/Unity5.3.2f1
UNITY_HOME=/Applications/Unity2017.1.0.f3
#add needed flags to the compilerCSHARP_FLAGS = -out:$(EXECUTABLE)
CSHARP_FLAGS = -unsafe -debug -checked- -langversion:4
# -doc:doc.xml
# CSHARP_FLAGS = -r:$(UNITY_HOME)/Unity.app/Contents/Frameworks/Mono/lib/mono/micro/mscorlib.dll

#change to the environment compiler
CSHARP_COMPILER = mcs
# CSHARP_COMPILER = $(UNITY_HOME)/Unity.app/Contents/Mono/bin/mcs
# CSHARP_COMPILER = $(UNITY_HOME)/Unity.app/Contents/MonoBleedingEdge/bin/mcs
LIB = $(UNITY_HOME)/Unity.app/Contents/MonoBleedingEdge/lib/mono/4.0
RUN_EXE = $(UNITY_HOME)/Unity.app/Contents/MonoBleedingEdge/bin/mono

#if needed, change the executable file
EXECUTABLE = $(MAIN_FILE).exe
LIBRARY = $(MAIN_FILE).dll
INSTALL_LOCATION = /Users/shane/unity/Eye\ Shader/Assets/Push3

all: $(LIBRARY) $(EXECUTABLE) Tests.dll

$(EXECUTABLE): $(LIBRARY)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -r:$(LIBRARY) PshGP.cs -out:$(EXECUTABLE) -main:$(MAIN_FILE)

# $(CSHARP_COMPILER) $(CSHARP_FLAGS) -r:$(LIBRARY) $(MAIN_FILE).cs -out:$(EXECUTABLE)

$(LIBRARY): $(CSHARP_SOURCE_FILES)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -target:library $(CSHARP_SOURCE_FILES) -out:$(LIBRARY)


Tests.dll: $(CSHARP_TEST_FILES) $(LIBRARY)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -target:library -r:NUnit.3.7.0/lib/net40/nunit.framework.dll -r:$(LIBRARY) $(CSHARP_TEST_FILES) -out:Tests.dll

test: Tests.dll
	echo hi

run: all
	$(RUN_EXE) ./$(EXECUTABLE) gpsamples/intreg1.pushgp

install: $(LIBRARY)
	cp $(LIBRARY) $(INSTALL_LOCATION)

doc:
	doxygen Doxyfile.txt

clean:
	$(RM) $(EXECUTABLE) $(LIBRARY) Tests.dll
