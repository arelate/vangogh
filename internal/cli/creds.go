// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package cli

import (
	"bufio"
	"fmt"
	"golang.org/x/crypto/ssh/terminal"
	"os"
	"strings"
	"syscall"
)

func Credentials(user string) (username string, password string, err error) {
	reader := bufio.NewReader(os.Stdin)

	if user == "" {
		fmt.Print("Enter username: ")
		username, _ = reader.ReadString('\n')
		username = strings.TrimSpace(username)
	} else {
		username = user
	}

	fmt.Printf("Enter password for %s:", username)
	bytePassword, err := terminal.ReadPassword(int(syscall.Stdin))
	if err != nil {
		return username, "", err
	}

	password = strings.TrimSpace(string(bytePassword))

	fmt.Println()

	return username, password, nil
}
