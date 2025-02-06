using System.Collections.Generic;
using UnityEngine;

public class RectangleSelector : MonoBehaviour
{
    public GridManager grid_manager;

    [SerializeField] private GameObject selector_box;
    private GameObject selector_box_instance;
    [SerializeField] private GameObject commit_button;

    private bool active;
    private (int row, int col) start_gpos;
    private (int row, int col) top_left;
    private (int row, int col) bot_right;
    private (int row, int col) size;
    private List<(int, int, char)> original_buffer;

    
    void Start()
    {
        active = false;
        start_gpos = (-1, -1);
        top_left = (-1, -1);
        bot_right = (-1, -1);
        size = (-1, -1);
        original_buffer = new List<(int, int, char)>();
    }

    void Update(){
        if (active){
            render_rectangle_selector();
        }
    }

    public void render_rectangle_selector(){
        selector_box_instance.GetComponent<RectTransform>().sizeDelta = new Vector2(
            grid_manager.get_col_size() * size.col,
            grid_manager.get_row_size() * size.row 
        );
        selector_box_instance.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            grid_manager.ui_manager.ui_panel_transform.rect.width + top_left.col * grid_manager.get_col_size(),
            grid_manager.invert_row_pos(bot_right.row) * grid_manager.get_row_size()
        );
    }

    public void initialize_selector_box(){
        selector_box_instance = Instantiate(
            selector_box,
            new Vector3(0, 0, 0),
            transform.rotation
        );
        selector_box_instance.transform.SetParent(grid_manager.canvas_transform);
    }

    public void destroy_selector_box(){

    }
    
    public void on_mouse_down((int row, int col) gpos){
        if (!active){
            start_gpos = (gpos.row, gpos.col);
            change_corners(gpos);
            initialize_selector_box();
            render_rectangle_selector();
        }
        else{
            start_gpos = (gpos.row, gpos.col);
        }

    }

    public void on_mouse_move((int row, int col) gpos){
        if (!active){
            change_corners(gpos);
            render_rectangle_selector();
        }
        else if (gpos != start_gpos){
            int row_delta = gpos.row - start_gpos.row;
            int col_delta = gpos.col - start_gpos.col;

            bound_deltas(top_left);
            bound_deltas(bot_right);

            top_left = (top_left.row + row_delta, top_left.col + col_delta);
            bot_right = (bot_right.row + row_delta, bot_right.col + col_delta); 
            start_gpos = gpos;
            if (row_delta != 0 || col_delta != 0){
                for(int x = 0; x < grid_manager.get_pbuffer_length(); x++){
                    grid_manager.edit_pbuffer_item_pos(x, row_delta, col_delta);
                }
            }

            void bound_deltas((int row, int col) corner){
                if (row_delta + corner.row < 0) {row_delta = corner.row * -1;}
                else if (row_delta + corner.row >= grid_manager.get_row_count()) {
                    row_delta = grid_manager.get_row_count() - 1 - corner.row;
                }
                if (col_delta + corner.col < 0) {col_delta = corner.col * -1;}
                else if (col_delta + corner.col >= grid_manager.get_col_count()) {
                    col_delta = grid_manager.get_col_count() - 1 - corner.col;
                }
            };
        }
    }

    public void on_mouse_up(){
        if (!active){
            active = true;
            commit_button.SetActive(true);
            for(int row = top_left.row; row <= bot_right.row; row++){
                for(int col = top_left.col; col <= bot_right.col; col++){
                    grid_manager.add_to_preview_buffer(row, col, grid_manager.get_garr_space(row, col));
                    original_buffer.Add((row, col, grid_manager.get_garr_space(row, col)));
                    grid_manager.add_to_grid_array(row, col, ' ');
                }
            } 
        }
    }

    public void reset_box(){
        active = false;
        top_left = (-1, -1);
        bot_right = (-1, -1);
        size = (-1, -1);
        start_gpos = (-1, -1);

        grid_manager.writePbufferToArray();
        Destroy(selector_box_instance);
        commit_button.SetActive(false);
    }

    public void change_corners((int row, int col) new_gpos){
        top_left = (
            Mathf.Min(start_gpos.row, new_gpos.row),
            Mathf.Min(start_gpos.col, new_gpos.col)
        );
        bot_right = (
            Mathf.Max(start_gpos.row, new_gpos.row),
            Mathf.Max(start_gpos.col, new_gpos.col)
        );
        size = (bot_right.row - top_left.row + 1, bot_right.col - top_left.col + 1); 
    }



}
