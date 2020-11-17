import {Component} from "@angular/core";

@Component({
  selector: 'app-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.scss']
})
export class ShellComponent {
  isSidenavOpen: boolean = false;

  onMenuClicked() {
    this.isSidenavOpen = !this.isSidenavOpen;
  }

  handleDrawerClosed() {
    this.isSidenavOpen = false;
  }
}
