import { Component, inject, input, OnInit } from '@angular/core';
import type { EChartsCoreOption } from 'echarts/core';
import { NgxEchartsDirective } from 'ngx-echarts';
import { provideEchartsCore } from 'ngx-echarts'
import * as echarts from 'echarts/core'
import { BarChart } from 'echarts/charts'
import { CanvasRenderer } from 'echarts/renderers';
import { GridComponent, LegendComponent, TitleComponent, TooltipComponent } from 'echarts/components'
import { NetApiService } from '../../../services/net-api/net-api.service';
import { PopupLoaderService } from '../../../services/popup-loader/popup-loader.service';
import { TopUserMostPoints } from '../../../models/stats-models';

echarts.use([
  TitleComponent,
  TooltipComponent,
  GridComponent,
  LegendComponent,
  BarChart,
  CanvasRenderer
])

@Component({
  selector: 'app-top-users-most-points',
  imports: [NgxEchartsDirective],
  templateUrl: './top-users-most-points.component.html',
  styleUrl: './top-users-most-points.component.css',
  providers: [
    provideEchartsCore({ echarts }),
    PopupLoaderService
  ]
})
export class TopUserMostPointsComponent implements OnInit {
  netApi = inject(NetApiService)
  popupLoader = inject(PopupLoaderService)
  option: EChartsCoreOption | null = null
  height = input<string>('300px')
  friendsOnly = input<boolean>(false)
  labelPosition = input<'top' | 'bottom'>('bottom')

  ngOnInit(): void {
    this.fetchChartData()
  }

  fetchChartData(): void {
    let action = 'top-users-most-points'
    if(this.friendsOnly()) action = 'top-friends-most-points'

    this.netApi
    .get<{users: TopUserMostPoints[]}>('Statistics', action)
    .subscribe({
        next: (r) => this.loadChartData(r.users),
        error: () => this.popupLoader.showPopup('Erro ao carregar top utilizadores com mais pontos.')
    })
  }

  loadChartData(data: TopUserMostPoints[]): void {
    // Order from highest to lowest
    data.sort((a, b) => b.points - a.points)

    this.option = {
      tooltip: {
        trigger: 'axis',
        axisPointer: {
          type: 'shadow'
        }
      },
      legend: {
        top: this.labelPosition()
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: {
        type: 'category',
        data: data.map(c => c.username),
        axisLabel: {
          rotate: 45,
          interval: 0
        }
      },
      yAxis: {
        type: 'value',
        minInterval: 1,
      },      
      series: [
        {
          name: 'Pontos',
          type: 'bar',
          data: data.map(c => c.points),
          itemStyle: {
            color: ({ dataIndex }: {dataIndex: number}) => {
              // Give gold silver and bronze to top 3
              const rankColors = ['#FFD700', '#C0C0C0', '#CD7F32']
              return rankColors[dataIndex] || '#7655ed'
            }
          }
        }
      ]
    }
  }
}
