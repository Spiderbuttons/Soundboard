<lane orientation="vertical" horizontal-content-alignment="middle" layout="80%" *context={Soundboard}>
    <grid *switch={SelectedCategory} layout="stretch content" item-layout="count: 2" item-spacing="32,0" horizontal-item-alignment="middle">
        <frame *case="Music" *repeat={Music} +state:playing={IsPlaying} transform-origin="0.5,0.5" +state:playing:transform="scale: 0.95" focusable="true" padding="24, 16" margin="0, 12" vertical-content-alignment="middle" background={@Spiderbuttons.Soundboard/Sprites/UI:Border}>
            <lane layout="stretch content" *switch={IsPlaying} margin="0,8" vertical-content-alignment="middle" click=|ToggleState()|>    
                <image *case="false" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                <image *case="true" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                <lane vertical-content-alignment="middle">
                    <label max-lines="1" margin="0,0,8,0" text={Id} />
                    <!-- Uncommenting the line below will cause an NRE when FormattedDuration is a field.
                        If FormattedDuration is a property instead (without Notify), it will hang the game. -->
                    <!-- <label margin="0,0,8,0" text={FormattedDuration} /> -->
                    <image *if={IsModded} layout="32px" sprite={@Spiderbuttons.Soundboard/Sprites/UI:Pufferchick} />
                </lane>
            </lane>
        </frame>
    </grid>
</lane>