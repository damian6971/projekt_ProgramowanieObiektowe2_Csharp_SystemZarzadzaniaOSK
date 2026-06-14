#include <stdio.h>
#include <stdlib.h>

/*
rozklad liczby naturalnej n na czynniki pierwsze

n = 4350
4350 / 2 = 2175  -> 2
2175 / 3 =  725  -> 3
 725 / 5 =  145  -> 5
 145 / 5 =   29  -> 5
  29 / 29 =   1  -> 29

rozklad: 2 3 5 5 29

tablica liczb pierwszych: 2 3 5 7 11 13 17 19 23 29 31 37 41 43 47 53 ...
*/

int primes[] = {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 0};

void rozklad(int n) {
    int i = 0;

    while (primes[i] != 0 && n > 1) {
        while (n % primes[i] == 0) {
            printf(" %d", primes[i]);
            n = n / primes[i];
        }
        i++;
    }

    if (n > 1) {
        printf(" %d", n);
    }
}

/*
- dokonaj analizy wywolania rozklad(12)

* rozklad(12)
  i = 0  primes[0] = 2
  12 % 2 = 0  print 2  n = 6
   6 % 2 = 0  print 2  n = 3
   3 % 2 = 1  break inner
  i = 1  primes[1] = 3
   3 % 3 = 0  print 3  n = 1
   1 <= 1  stop

  rozklad: 2 2 3

- dokonaj analizy wywolania rozklad(4350)

* rozklad(4350)
  i = 0  primes[0] = 2
  4350 % 2 = 0  print 2  n = 2175
  2175 % 2 = 1  break inner
  i = 1  primes[1] = 3
  2175 % 3 = 0  print 3  n = 725
   725 % 3 = 2  break inner
  i = 2  primes[2] = 5
   725 % 5 = 0  print 5  n = 145
   145 % 5 = 0  print 5  n = 29
    29 % 5 = 4  break inner
  i = 3  primes[3] = 7
    29 % 7 = 1  break inner
  ...
  i = 9  primes[9] = 29
    29 % 29 = 0  print 29  n = 1
     1 <= 1  stop

  rozklad: 2 3 5 5 29
*/

int main() {
    printf("rozklad.c\n\n");

    int n;

    printf("n = ");
    scanf("%d", &n);

    printf("rozklad:");
    rozklad(n);
    printf("\n");

    return 0;
}
