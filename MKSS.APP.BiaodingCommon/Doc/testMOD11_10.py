 
def getPn(n, arr1):
    if n == 1:
        return 10
    else:
        return mod10(getSn(n - 1, arr1)) * 2
 
def mod10(num):
    if num % 10 == 0:
        return 10
    else:
        return num % 10
 
def getSn(n, arr1):
    return getPn(n, arr1) % 11 + int(arr1[14-n+1])
 
def getCheckCode(code):
    c = code + 'x,'
    arr1 = []
    for i in reversed(c):
        arr1.append(i)
    for j in range(0, 10):
        arr1[1] = str(j)
        if getSn(14, arr1) % 10 == 1:
            result = ''.join(list(reversed(arr1)))
            return result[:len(result) - 1]

if __name__ == '__main__': 
    xkzCode = '1530825001410' 
    print(getCheckCode(xkzCode)) 