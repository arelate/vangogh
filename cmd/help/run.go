// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package help

import (
	"fmt"
	"github.com/boggydigital/vangogh/internal/strings/cmds"
)

func Run(args []string) error {
	fmt.Println(cmds.Help, args)
	return nil
}
