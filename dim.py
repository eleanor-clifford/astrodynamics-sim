N = 6
Z = 4

for s in range(N):
    for n in range(N):
        if (n-s) < 0:
            print("0"*Z,end=" ")
        else:
            print(str(int(2**(n-s)*(n-s+1)**s)).zfill(Z),end=" ")
    print()