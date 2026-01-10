from math import dist

MolFileName = "MolView.mol"
LogFileName = "output/molecule.json"

MolFile = open(MolFileName, "r")

MolList = []
BondList = []
MolLines = []
BondLines = []

BondCounter = {}


def Confirmation(prompt):
    a="0"
    aList = ("y", "n")
    while a.lower not in aList:
        a = input(prompt+"(y/n)")
        if a == "y":
            return True
        if a == "n":
            return False

def ReadMolFile():
    global MolList

    for line in MolFile:
        rowList = line.split(" ")
        if len(rowList) <= 3:
            continue
        while "" in rowList:
            rowList.remove("")
        rowList[-1] = rowList[-1].split("\n")[0]

        if len(rowList) == 16:
            MolList.append(rowList)
        elif len(rowList) == 6 or len(rowList) == 7:
            BondList.append(rowList)


def BuildMolChunk():
    cordList = []
    distList = []
    transSet = ()

    hasOxygen = False

    AddHToOxygen = False

    for entry in MolList:
        x = float(entry[0])
        y = float(entry[1])

        distXY = dist((x, y), (0, 0))

        cordList.append([distXY, x, y])
        distList.append(distXY)

        if entry[3] == "O":
            hasOxygen = True

    distList.sort()
    for cords in cordList:
        if cords[0] == distList[0]:
            transSet = (cords[1], cords[2])

    if hasOxygen:
        AddHToOxygen = Confirmation("Would you like to add implicit Hydrogen to Oxygen?")

    count = 0
    for entry in MolList:
        x = round(float(entry[0]) - transSet[0], 4)
        y = round(float(entry[1]) - transSet[1], 4)
        element = entry[3]

        MolChunkLines = ['\t{', '"type": "atom",', '"index": ' + str(count) + ',', '"x": ' + str(x) + ',', '"y": ' + str(y)]
        if element != "C":
            elementLine = '"element": "' + element + '",'
            MolChunkLines.insert(2, elementLine)

            if (element == "O") & AddHToOxygen:
                if BondCounter[str(count)] < 2:
                    MolChunkLines.insert(3, '"right": "H",')

        MolChunkLines = ['\t' + entry for entry in MolChunkLines]

        MolChunkLines.append('},')

        MolChunk = "\n\t\t".join(MolChunkLines) + "\n"



        MolLines.append(MolChunk)
        #print(MolChunk)

        count += 1


def BuildBondChunk():
    for entry in BondList:
        a = str(int(entry[0]) - 1)
        b = str(int(entry[1]) - 1)
        type = []

        if int(entry[2]) < 4:
            for i in range(int(entry[2])):
                type.append('"solid",')
            type[-1] = type[-1][:-1]
        else:
            type =['"solid",', '"dotted"']

        sterochem = int(entry[3])
        if sterochem != 0:
            match sterochem:
                case 1:
                    type = ['"outward"']
                case 6:
                    type = ['"inward"']

        type = ['\t' + bond for bond in type]

        BondChunkLines = ['\t{', '"type": "bond",', '"a": ' + a + ',', '"b": ' + b + ',', '"lines": [']
        BondChunkLines.extend(type)
        BondChunkLines.append(']')

        if (MolList[int(a)][3] == "O" or MolList[int(b)][3] == "O") and entry[2] == "2":
            BondChunkLines[-1] = BondChunkLines[-1] + ','
            BondChunkLines.append('"centered": true')

        BondChunkLines = ['\t' + entry for entry in BondChunkLines]

        BondChunkLines.append('},')

        BondChunk = "\n\t\t".join(BondChunkLines) + "\n"

        BondLines.append(BondChunk)

        CountBonds([a,b], len(type))

def CountBonds(bondPair, bondNum):
    for atom in bondPair:
        if atom in BondCounter:
            BondCounter[atom] = BondCounter[atom] + bondNum
        else:
            BondCounter[atom] = bondNum

def BuildJson():
    material = input("GTMaterial ID:")
    LogFileName = "outputs/" + material + ".json"

    LogFile = open(LogFileName, "w")
    header = '{\n\t"contents": [\n'
    footer = '\t]\n}'

    LogFile.write(header)
    LogFile.writelines(MolLines)
    LogFile.writelines(BondLines)
    LogFile.write(footer)

ReadMolFile()
BuildBondChunk()
BuildMolChunk()

print(MolList)
print(BondList)
print(BondCounter)

BondLines[-1] = BondLines[-1][:-2] + "\n"

BuildJson()


