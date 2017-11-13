#change this to the name of the Main class file, without file extension
MAIN_FILE = PshGP

#change this to the depth of the project folders
#if needed, add a preffix for a common project folder
CSHARP_SOURCE_FILES = $(wildcard src/*/*/*.cs src/*/*.cs src/*.cs)
CSHARP_TEST_FILES = $(wildcard tests/*/*/*.cs tests/*/*.cs tests/*.cs)

# UNITY_HOME=/Applications/Unity5.3.2f1
UNITY_HOME=/Applications/Unity2017.1.0.f3
#add needed flags to the compilerCSHARP_FLAGS = -out:$(EXECUTABLE)
CSHARP_FLAGS = -unsafe -debug -checked- -langversion:4
# -doc:doc.xml
# CSHARP_FLAGS = -r:$(UNITY_HOME)/Unity.app/Contents/Frameworks/Mono/lib/mono/micro/mscorlib.dll

#change to the environment compiler
CSHARP_COMPILER = mcs
MONO = mono
# CSHARP_COMPILER = $(UNITY_HOME)/Unity.app/Contents/Mono/bin/mcs
# CSHARP_COMPILER = $(UNITY_HOME)/Unity.app/Contents/MonoBleedingEdge/bin/mcs
LIB = $(UNITY_HOME)/Unity.app/Contents/MonoBleedingEdge/lib/mono/4.0
RUN_EXE = $(UNITY_HOME)/Unity.app/Contents/MonoBleedingEdge/bin/mono

#if needed, change the executable file
EXECUTABLE = $(MAIN_FILE).exe
LIBRARY = $(MAIN_FILE).dll
INSTALL_LOCATION = /Users/shane/unity/Eye\ Shader/Assets/Push3
NUNIT_DIR = NUnit.Framework-3.8.1/bin/net-4.0

all: $(LIBRARY) $(EXECUTABLE) Tests.dll PshInspector.exe

# I thought it'd be nice to not recompile everything.  But it's just a hassle so forget it.
# $(EXECUTABLE): $(LIBRARY) PshGP.cs
# 	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -r:$(LIBRARY) PshGP.cs -out:$(EXECUTABLE) -main:$(MAIN_FILE)

# $(EXECUTABLE): $(CSHARP_SOURCE_FILES) PshGP.cs
# 	$(CSHARP_COMPILER) $(CSHARP_FLAGS) $(CSHARP_SOURCE_FILES) PshGP.cs -out:$(EXECUTABLE) -main:$(MAIN_FILE)

%.exe: %.cs $(CSHARP_SOURCE_FILES)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) $(CSHARP_SOURCE_FILES) $< -out:$@ -main:$(basename $(notdir $<))

$(LIBRARY): $(CSHARP_SOURCE_FILES)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -target:library $(CSHARP_SOURCE_FILES) -out:$(LIBRARY)


# Tests.dll: $(CSHARP_TEST_FILES) $(LIBRARY)
# 	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -target:library -r:$(NUNIT_DIR)/nunit.framework.dll -r:$(LIBRARY) $(CSHARP_TEST_FILES) -out:Tests.dll

Tests.dll: $(CSHARP_TEST_FILES) $(CSHARP_SOURCE_FILES)
	$(CSHARP_COMPILER) $(CSHARP_FLAGS) -target:library -r:$(NUNIT_DIR)/nunit.framework.dll $(CSHARP_SOURCE_FILES) $(CSHARP_TEST_FILES) -out:Tests.dll

test: Tests.dll
	$(MONO) --debug $(NUNIT_DIR)/nunitlite-runner.exe -noresult -noheader Tests.dll

sample-run: all
	$(RUN_EXE) ./$(EXECUTABLE) gpsamples/intreg1.pushgp

install: $(LIBRARY)
	cp $(LIBRARY) $(INSTALL_LOCATION)

doc:
	doxygen Doxyfile.txt

clean:
	$(RM) $(EXECUTABLE) $(LIBRARY) Tests.dll
