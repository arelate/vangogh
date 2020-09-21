// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package help

func Intercept(cmd string, args []string) (bool, error) {
	if len(args) < 1 {
		args = make([]string, 1)
		args[0] = cmd
		err := Run(args)
		if err != nil {
			return true, err
		}
	}
	return false, nil
}
