package cli

import (
	"bufio"
	"fmt"
	"golang.org/x/crypto/ssh/terminal"
	"os"
	"strings"
	"syscall"
)

func Credentials() (username string, password string, err error) {
	reader := bufio.NewReader(os.Stdin)

	fmt.Print("Enter username: ")
	username, _ = reader.ReadString('\n')
	username = strings.TrimSpace(username)

	fmt.Print("Enter password:")
	bytePassword, err := terminal.ReadPassword(int(syscall.Stdin))
	if err != nil {
		return username, "", err
	}

	password = strings.TrimSpace(string(bytePassword))

	return username, password, nil
}
