import {Component, Input} from "@angular/core";
import {HausEvent} from "../../../shared/models";

@Component({
  selector: 'recent-events',
  templateUrl: './recent-events.component.html',
  styleUrls: ['./recent-events.component.scss']
})
export class RecentEventsComponent {
  @Input() events: Array<HausEvent> | null = [];
}
