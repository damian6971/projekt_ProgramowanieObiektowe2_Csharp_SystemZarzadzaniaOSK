         [bits 32]

;        esp -> [ret]  ; ret - adres powrotu do asmloader

%ifdef COMMENT
rozklad liczby naturalnej n na czynniki pierwsze

n = 4350
4350 / 2 = 2175  -> 2
2175 / 3 =  725  -> 3
 725 / 5 =  145  -> 5
 145 / 5 =   29  -> 5
  29 / 29 =   1  -> 29

rozklad: 2 3 5 5 29

tablica liczb pierwszych: 2 3 5 7 11 13 17 19 23 29 31 37 41 43 47 53 ...
%endif

         mov ebp, ebx  ; ebp = ebx  ; store ebx

         sub esp, 4    ; zarezerwuj miejsce na n

;        esp -> [n][ret]

         call getaddr1  ; push on the stack the run-time address of format1 and jump to getaddr1
format1:
         db "n = ", 0
getaddr1:

;        esp -> [format1][n][ret]

         call [ebp+3*4]  ; printf("n = ");
         add esp, 4      ; esp = esp + 4

;        esp -> [n][ret]

         push esp  ; push addr_n

;        esp -> [addr_n][n][ret]

         call getaddr2  ; push on the stack the run-time address of format2 and jump to getaddr2
format2:
         db "%d", 0
getaddr2:

;        esp -> [format2][addr_n][n][ret]

         call [ebp+4*4]  ; scanf("%d", addr_n);
         add esp, 2*4    ; esp = esp + 8

;        esp -> [n][ret]

         pop edi  ; edi = n

;        esp -> [ret]

         call getaddr3  ; push on the stack the run-time address of format3 and jump to getaddr3
format3:
         db "rozklad:", 0
getaddr3:

;        esp -> [format3][ret]

         call [ebp+3*4]  ; printf("rozklad:");
         add esp, 4      ; esp = esp + 4

;        esp -> [ret]

         call get_primes  ; push on the stack the run-time address of primes and jump to get_primes
primes:
         dd 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 0
get_primes:

;        esp -> [primes][ret]

         pop esi  ; esi = addr of primes

;        esp -> [ret]

outer    mov ecx, [esi]  ; ecx = primes[i]
         test ecx, ecx   ; ecx & ecx        ; OF=0 SF ZF PF CF=0 affected
         jz done         ; jump if zero      ; jump if ZF = 1

         cmp edi, 1      ; edi - 1           ; OF SF ZF AF PF CF affected
         jbe done        ; jump if below or equal  ; jump if CF = 1 or ZF = 1

inner    xor edx, edx  ; edx = 0
         mov eax, edi  ; eax = n
         div ecx       ; eax = n / prime, edx = n % prime

         test edx, edx  ; edx & edx        ; OF=0 SF ZF PF CF=0 affected
         jnz next       ; jump if not zero  ; jump if ZF = 0

         mov edi, eax   ; n = n / prime

         push ecx  ; ecx -> stack = prime

;        esp -> [prime][ret]

         call getaddr4  ; push on the stack the run-time address of format4 and jump to getaddr4
format4:
         db " %d", 0
getaddr4:

;        esp -> [format4][prime][ret]

         call [ebp+3*4]  ; printf(" %d", prime);
         add esp, 2*4    ; esp = esp + 8

;        esp -> [ret]

         mov ecx, [esi]  ; ecx = primes[i]  ; reload after printf
         jmp inner       ; divide by same prime again

next     add esi, 4  ; esi = esi + 4  ; next prime
         jmp outer

done     cmp edi, 1  ; edi - 1           ; OF SF ZF AF PF CF affected
         jbe finish  ; jump if below or equal  ; jump if CF = 1 or ZF = 1

         push edi  ; edi -> stack = n

;        esp -> [n][ret]

         call getaddr5  ; push on the stack the run-time address of format5 and jump to getaddr5
format5:
         db " %d", 0
getaddr5:

;        esp -> [format5][n][ret]

         call [ebp+3*4]  ; printf(" %d", n);
         add esp, 2*4    ; esp = esp + 8

;        esp -> [ret]

finish   call getaddr6  ; push on the stack the run-time address of format6 and jump to getaddr6
format6:
         db 0xA, 0
getaddr6:

;        esp -> [format6][ret]

         call [ebp+3*4]  ; printf("\n");
         add esp, 4      ; esp = esp + 4

;        esp -> [ret]

         push 0          ; esp -> [00 00 00 00][ret]
         call [ebp+0*4]  ; exit(0);

; asmloader API
;
; ESP wskazuje na prawidlowy stos
; argumenty funkcji wrzucamy na stos
; EBX zawiera pointer na tablice API
;
; call [ebx + NR_FUNKCJI*4] ; wywolanie funkcji API
;
; NR_FUNKCJI:
;
; 0 - exit
; 1 - putchar
; 2 - getchar
; 3 - printf
; 4 - scanf
;
; To co funkcja zwroci jest w EAX.
; Po wywolaniu funkcji sciagamy argumenty ze stosu.
;
; https://gynvael.coldwind.pl/?id=387

%ifdef COMMENT

ebx    -> [ ][ ][ ][ ] -> exit
ebx+4  -> [ ][ ][ ][ ] -> putchar
ebx+8  -> [ ][ ][ ][ ] -> getchar
ebx+12 -> [ ][ ][ ][ ] -> printf
ebx+16 -> [ ][ ][ ][ ] -> scanf

%endif
